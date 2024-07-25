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
        Damaged,
        Die
    }

    // 공격 패턴
    enum AttackPattern
    {
        Melee,
        Shot
    }

    // 공격 패턴
    AttackPattern pattern;
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
    // 이동 방향 
    Vector3 dir;
    // 보스 공격력
    public int attackPower = 3;
    // 이동속도
    public float moveSpeed = 5;

    // 근접 공격 범위
    public float meleeAttackDistance = 2f;
    // 근접 공격력
    public int meleeAttackPower = 10;
    // 원거리 공격 범위
    public float shotAttackDistance = 10f;
    // 원거리 공격력
    public int shotAttackPower = 5;

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
        // 플레이어가 있는 방향으로 몸을 회전시킨다.
        Vector3 directionToPlayer = player.position - transform.position;
        directionToPlayer.y = 0;
        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
        
        // 보간을 이용하여 속도 조절
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
    // 대기 상태 함수
    void Idle()
    {
        // 플레이어와의 거리가 인지범위 안에 들어오면, Move 상태로 전환한다.
        float dist = Vector3.Distance(player.transform.position, transform.position);
        if(findDistance > dist)
        {
            // move 상태로 변환
            state = EnemyState.Move;
        }
    }

    void Move()
    {
        // 플레이어와의 거리가 공격 범위 밖이라면 플레이어를 향해 이동한다.
        float dist = Vector3.Distance(player.transform.position, transform.position);

        // 적이 플레이어와의 거리가 근접 공격 범위 이내에 있으면 공격 상태로 전환
        if (dist < meleeAttackDistance)
        {
            state = EnemyState.Attack;
            // 이동 벡터를 0으로 설정하여 이동을 멈춘다.
            cc.Move(Vector3.zero);
            return;
        }

        // 플레이어 방향으로 향한다.
        dir = player.transform.position - transform.position;
        dir.Normalize();
        dir.y = 0;
        // 캐릭터 컨트롤러를 이용하여 이동
        cc.Move(dir * moveSpeed * Time.deltaTime);

        //if(attackDistance < dist)
        //{
        //    dir = player.transform.position - transform.position;
        //    dir.Normalize();
        //    // 캐릭터 컨트롤러를 이용해 이동한다.
        //    cc.Move(dir * moveSpeed * Time.deltaTime);
        //}
        //// 플레이어와의 거리가 공격 범위 안이라면 공격한다.
        //else
        //{
        //    state = EnemyState.Attack;
        //    // 누적 시간을 공격 딜레이 시간만큼 미리 진행시켜 놓는다.
        //    currTime = attackDelayTime;
        //}
        //// 제자리에서 플레이어 방향으로 드릴 내밀며 몸 회전 (정지상태)
        //// 플레이어가 방향을 바꾸면 플레이어를 따라 몸을 회전시킴.
    }

    void Attack()
    {
        // 만약, 플레이어가 공격 범위 이내에 있다면 플레이어를 공격한다.
        float dist = Vector3.Distance(player.transform.position, transform.position);
        // 공격 패턴에 따른 공격 범위 및 처리
        // 근거리
        if(dist < meleeAttackDistance && pattern == AttackPattern.Melee)
        {
            currTime += Time.deltaTime;
            if(currTime >= attackDelayTime)
            {
                MeleeAttack();
                currTime = 0;
            }
        }
        // 원거리
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
        //    // 일정 시간마다 플레이어를 공격한다.
        //    currTime += Time.deltaTime;
        //    if (currTime > attackDelayTime)
        //    {
        //        print("공격");
        //        currTime = 0;
        //    }
        //}
        //// 그렇지 않다면, 현재 상태를 Move로 전환한다(재추격)
        //else
        //{
        //    // 플레이어 컴포넌트
        //    state = EnemyState.Move;
        //    currTime = 0;
        //}
    }
    // 공격 패턴 함수
    void UpdatePattern()
    {
        if(/* 조건 */true)
        {
            pattern = AttackPattern.Melee;
        }
        else
        {
            pattern = AttackPattern.Shot;
        }
    }
    // 근접 공격
    void MeleeAttack()
    {
        print("근접 공격");
    }
    
    // 원거리 공격
    void ShotAttack()
    {
        print("원거리 공격");
    }

    void Damaged()
    {

    }

    void Die()
    {

    }
}
