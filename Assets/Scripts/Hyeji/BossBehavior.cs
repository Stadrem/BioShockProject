using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBehavior : MonoBehaviour
{
    // 에너미 상태
    enum EnemyState
    {
        Idle,
        Move,
        Attack,
        Return,
        Damaged,
        Die
    }
    // 에너미 상태 변수
    EnemyState state;
    // 플레이어 발견 범위
    public float findDistance = 8f;
    // 플레이어 공격 가능 범위
    public float attackDistance = 2f;
    // Player Transform
    Transform player;
    // 캐릭터 컨트롤러 컴포넌트
    CharacterController cc;
    // 현재 시간
    float currTime = 0;
    // 공격 딜레이 시간
    float attackDelayTime = 2f;

    void Start()
    {
        // 최초의 보스 상태는 Idle
        state = EnemyState.Idle;
        // Player의 Transform 컴포넌트 받아오기
        player = GameObject.Find("Player").transform;
        // Boss의 캐릭터 컨트롤러 컴포넌트 받아오기
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

    // 대기 상태 함수
    void Idle()
    {
        // 플레이어와의 거리가 인지범위 안에 들어오면, Move 상태로 전환한다.
        float dist = Vector3.Distance(player.transform.position, transform.position);
        if(findDistance > dist)
        {
            state = EnemyState.Move;
        }
    }

    void Move()
    {
        // 오른손(드릴)을 내밀며 움직이며 포효한다.
        // 제자리에서 플레이어 방향으로 드릴 내밀며 몸 회전 (정지상태)
        // 플레이어가 방향을 바꾸면 플레이어를 따라 몸을 회전시킴.
    }

    void Attack()
    {
        // 만약, 플레이어가 공격 범위 이내에 있다면 플레이어를 공격한다.
        float dist = Vector3.Distance(player.transform.position, transform.position);
        if(dist < attackDistance)
        {

        }
        // 그렇지 않다면, 현재 상태를 Move로 전환한다(재추격)
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
