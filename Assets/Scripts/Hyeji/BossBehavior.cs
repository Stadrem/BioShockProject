using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBehavior : MonoBehaviour
{
    // ���ʹ� ����
    enum EnemyState
    {
        Idle,
        Move,
        Attack,
        Damaged,
        Die
    }

    // ���� ����
    enum AttackPattern
    {
        Melee,
        Shot
    }

    // ���� ����
    AttackPattern pattern;
    // ���ʹ� ���� ����
    EnemyState state;
    // �÷��̾� �߰� ����
    public float findDistance = 8f;
    // �÷��̾� ���� ���� ����
    public float attackDistance = 2f;
    // Player Transform
    Transform player;
    // ĳ���� ��Ʈ�ѷ� ������Ʈ
    CharacterController cc;
    // ���� �ð�
    float currTime = 0;
    // ���� ������ �ð�
    float attackDelayTime = 2f;
    // �̵� ���� 
    Vector3 dir;
    // ���� ���ݷ�
    public int attackPower = 3;
    // �̵��ӵ�
    public float moveSpeed = 5;

    // ���� ���� ����
    public float meleeAttackDistance = 2f;
    // ���� ���ݷ�
    public int meleeAttackPower = 10;
    // ���Ÿ� ���� ����
    public float shotAttackDistance = 10f;
    // ���Ÿ� ���ݷ�
    public int shotAttackPower = 5;

    void Start()
    {
        // ������ ���� ���´� Idle
        state = EnemyState.Idle;
        // Player�� Transform ������Ʈ �޾ƿ���
        player = GameObject.Find("Player").transform;
        // Boss�� ĳ���� ��Ʈ�ѷ� ������Ʈ �޾ƿ���
        cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        // �÷��̾ �ִ� �������� ���� ȸ����Ų��.
        Vector3 directionToPlayer = player.position - transform.position;
        directionToPlayer.y = 0;
        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
        
        // ������ �̿��Ͽ� �ӵ� ����
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 1f);

        switch (state)
        {
            case EnemyState.Idle:
                Idle();
                break;
            case EnemyState.Move:
                Move();
                break;
            case EnemyState.Attack:
                Attack();
                break;
            case EnemyState.Damaged:
                Damaged();
                break;
            case EnemyState.Die:
                Die();
                break;
        }
    }
    // ��� ���� �Լ�
    void Idle()
    {
        // �÷��̾���� �Ÿ��� �������� �ȿ� ������, Move ���·� ��ȯ�Ѵ�.
        float dist = Vector3.Distance(player.transform.position, transform.position);
        if(findDistance > dist)
        {
            // move ���·� ��ȯ
            state = EnemyState.Move;
        }
    }

    void Move()
    {
        // �÷��̾���� �Ÿ��� ���� ���� ���̶�� �÷��̾ ���� �̵��Ѵ�.
        float dist = Vector3.Distance(player.transform.position, transform.position);

        // ���� �÷��̾���� �Ÿ��� ���� ���� ���� �̳��� ������ ���� ���·� ��ȯ
        if (dist < meleeAttackDistance)
        {
            state = EnemyState.Attack;
            // �̵� ���͸� 0���� �����Ͽ� �̵��� �����.
            cc.Move(Vector3.zero);
            return;
        }

        // �÷��̾� �������� ���Ѵ�.
        dir = player.transform.position - transform.position;
        dir.Normalize();
        dir.y = 0;
        // ĳ���� ��Ʈ�ѷ��� �̿��Ͽ� �̵�
        cc.Move(dir * moveSpeed * Time.deltaTime);

        //if(attackDistance < dist)
        //{
        //    dir = player.transform.position - transform.position;
        //    dir.Normalize();
        //    // ĳ���� ��Ʈ�ѷ��� �̿��� �̵��Ѵ�.
        //    cc.Move(dir * moveSpeed * Time.deltaTime);
        //}
        //// �÷��̾���� �Ÿ��� ���� ���� ���̶�� �����Ѵ�.
        //else
        //{
        //    state = EnemyState.Attack;
        //    // ���� �ð��� ���� ������ �ð���ŭ �̸� ������� ���´�.
        //    currTime = attackDelayTime;
        //}
        //// ���ڸ����� �÷��̾� �������� �帱 ���и� �� ȸ�� (��������)
        //// �÷��̾ ������ �ٲٸ� �÷��̾ ���� ���� ȸ����Ŵ.
    }

    void Attack()
    {
        // ����, �÷��̾ ���� ���� �̳��� �ִٸ� �÷��̾ �����Ѵ�.
        float dist = Vector3.Distance(player.transform.position, transform.position);
        // ���� ���Ͽ� ���� ���� ���� �� ó��
        // �ٰŸ�
        if(dist < meleeAttackDistance && pattern == AttackPattern.Melee)
        {
            currTime += Time.deltaTime;
            if(currTime >= attackDelayTime)
            {
                MeleeAttack();
                currTime = 0;
            }
        }
        // ���Ÿ�
        else if(dist < shotAttackDistance && pattern == AttackPattern.Shot)
        {
            currTime += Time.deltaTime;
            if(currTime >= attackDelayTime)
            {
                ShotAttack();
                currTime = 0;
            }
        }
        else
        {
            state = EnemyState.Move;
        }

        //if (dist < attackDistance)
        //{
        //    // ���� �ð����� �÷��̾ �����Ѵ�.
        //    currTime += Time.deltaTime;
        //    if (currTime > attackDelayTime)
        //    {
        //        print("����");
        //        currTime = 0;
        //    }
        //}
        //// �׷��� �ʴٸ�, ���� ���¸� Move�� ��ȯ�Ѵ�(���߰�)
        //else
        //{
        //    // �÷��̾� ������Ʈ
        //    state = EnemyState.Move;
        //    currTime = 0;
        //}
    }
    // ���� ���� �Լ�
    void UpdatePattern()
    {
        if(/* ���� */true)
        {
            pattern = AttackPattern.Melee;
        }
        else
        {
            pattern = AttackPattern.Shot;
        }
    }
    // ���� ����
    void MeleeAttack()
    {
        print("���� ����");
    }
    
    // ���Ÿ� ����
    void ShotAttack()
    {
        print("���Ÿ� ����");
    }

    void Damaged()
    {

    }

    void Die()
    {

    }
}
