using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossBehavior : MonoBehaviour
{
    // ���ʹ� ����
    public enum EnemyState
    {
        Idle,
        Move,
        Attack,
        Damaged,
        Die
    }
    // ���ʹ� ���� ����
    public EnemyState state;
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
    public float moveSpeed = 2;

    // ���� ���� ����
    public float meleeAttackDistance = 2f;
    // ���� ���ݷ�
    public int meleeAttackPower = 10;
    // �߰Ÿ� ���� ����
    public float shotAttackDistance = 7f;
    // �߰Ÿ� ���ݷ�
    public int shotAttackPower = 5;
    // ������ ������
    public Transform rightHand;
    // ȸ���Ұ��ΰ�?
    bool isRoatate = false;
    // ȸ���ӵ�
    public float rotationSpeed = 2f;
    // ȸ�� �� ��� �ð�
    public float pauseDuration = 1f;
    // ���� ȸ����
    private Quaternion originalRotation;
    // Ÿ�� ȸ����
    private Quaternion targetRotation;
    // �÷��̾ ����?
    bool isPlayerClose = false;

    // ���� �ӵ�
    public float chargeSpeed = 10f;
    // ���� ���� �Ÿ�
    public float chargeRange = 15f;
    // ���� ����
    public bool isCharging = false;
    // ĳ������ ���� ����
    bool isMoving = false;

    void Start()
    {
        // ������ ���� ���´� Idle
        state = EnemyState.Idle;
        // Player�� Transform ������Ʈ �޾ƿ���
        player = GameObject.Find("Player").transform;
        // Boss�� ĳ���� ��Ʈ�ѷ� ������Ʈ �޾ƿ���
        cc = GetComponent<CharacterController>();
        // �ʱ� ȸ�� ���� ����
        originalRotation = rightHand.rotation;
        targetRotation = Quaternion.Euler(-50, -47, 54) * originalRotation;
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
        // �÷��̾�� ������ �Ÿ� ���ϱ�
        float dist = Vector3.Distance(player.transform.position, transform.position);

        // ���� �÷��̾���� �Ÿ��� ���� ���� ���� �̳��� ������ ���� ���·� ��ȯ
        if (dist < meleeAttackDistance)
        {
            state = EnemyState.Attack;
            // �̵� ���͸� 0���� �����Ͽ� �̵��� �����.
            cc.Move(Vector3.zero);
            return;
        }

        // �÷��̾���� �Ÿ��� ���� ���� ���̶�� �÷��̾� �������� ���Ѵ�.
        dir = player.transform.position - transform.position;
        dir.Normalize();
        dir.y = 0;
        // ĳ���� ��Ʈ�ѷ��� �̿��Ͽ� �̵�
        cc.Move(dir * moveSpeed * Time.deltaTime);
    }

    // ���� �Լ�
    void Attack()
    {
        // ����, �÷��̾ ���� ���� �̳��� �ִٸ� �÷��̾ �����Ѵ�.
        float dist = Vector3.Distance(player.transform.position, transform.position);

        // ���� ���Ͽ� ���� ���� ���� �� ó��
        // �ٰŸ�
        if(dist < meleeAttackDistance)
        {
            currTime += Time.deltaTime;
            if(currTime >= attackDelayTime)
            {
                MeleeAttack();
                currTime = 0;
            }
        }
        // �߰Ÿ�
        else if(dist < shotAttackDistance)
        {
            currTime += Time.deltaTime;
            if(currTime >= attackDelayTime)
            {
                RandomShotAttack();
                currTime = 0;
            }
        }
        else
        {
            state = EnemyState.Move;
        }
    }
    // ���� ����
    void MeleeAttack()
    {
        // �������� ȸ����Ų��.
        isRoatate = true;
        rightHand.rotation = targetRotation;

        // �÷��̾�� �־�����
        if(isPlayerClose)
        {
            // �������� ���� ��ġ�� �Ѵ�.
            rightHand.rotation = originalRotation;
        }

        // ������ ���� �帱�� �̿��Ͽ� �ķ�ģ��. ������ �� �밢��, �Ʒ� �밢�� 2���� �����ϰ� �ο�
        print("���� ����");
    }
    
    // �߰Ÿ� ���� ����
    void RandomShotAttack()
    {
        // �߰Ÿ� ���� 2���� �����ϰ� �ο�
        int attackType = Random.Range(0, 2);

        if(attackType == 0)
        {
            ShotAttackType1();
        }
        else
        {
            ShotAttackType2();
        }
    }

    // �߰Ÿ� ����1 - ���� ����
    void ShotAttackType1()
    {
        // �÷��̾���� �Ÿ� 
        float dist = Vector3.Distance(player.transform.position, transform.position);
        // �÷��̾���� �Ÿ��� ���� ���� ���� ũ�ٸ�
        if (dist < chargeRange)
        {
            if (!isCharging)
            {
                print("���� ����");
                StartCoroutine(ChargeTowardsPlayer());
            }
        }
        // �������� , ������ ���� ���� 2���� �����ϰ� �ο�

    }

    // �߰Ÿ� ����2 - ������ ����(������ġ��)
    void ShotAttackType2()
    {
        print("������ġ��");
    }

    // �÷��̾ ���� ����, ���� �ð��� ������ �̵� ���·� ���ư���.
    private IEnumerator ChargeTowardsPlayer()
    {
        isCharging = true;
        this.moveSpeed = chargeSpeed;

        float chargeDutation = 1f;
        float startTime = Time.time;

        while(Time.time < startTime + chargeDutation)
        {
            print("������");

            yield return null;
        }

        this.moveSpeed = moveSpeed;
        isCharging = false;
        state = EnemyState.Move;
    }

    void Damaged()
    {

    }

    void Die()
    {

    }
}
