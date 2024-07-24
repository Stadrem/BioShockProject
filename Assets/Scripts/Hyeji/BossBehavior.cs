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
        Return,
        Damaged,
        Die
    }
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
            case EnemyState.Return:
                Return();
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
            state = EnemyState.Move;
        }
    }

    void Move()
    {
        // ������(�帱)�� ���и� �����̸� ��ȿ�Ѵ�.
        // ���ڸ����� �÷��̾� �������� �帱 ���и� �� ȸ�� (��������)
        // �÷��̾ ������ �ٲٸ� �÷��̾ ���� ���� ȸ����Ŵ.
    }

    void Attack()
    {
        // ����, �÷��̾ ���� ���� �̳��� �ִٸ� �÷��̾ �����Ѵ�.
        float dist = Vector3.Distance(player.transform.position, transform.position);
        if(dist < attackDistance)
        {

        }
        // �׷��� �ʴٸ�, ���� ���¸� Move�� ��ȯ�Ѵ�(���߰�)
    }

    void Return()
    {

    }

    void Damaged()
    {

    }

    void Die()
    {

    }
}
