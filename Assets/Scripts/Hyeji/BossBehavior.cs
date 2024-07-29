using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossBehavior : MonoBehaviour
{
    // 에너미 상태
    public enum EnemyState
    {
        Idle,
        Move,
        Attack,
        Damaged,
        Die
    }
    // 에너미 상태 변수
    public EnemyState state;
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
    public float moveSpeed = 2;

    // 근접 공격 범위
    public float meleeAttackDistance = 2f;
    // 근접 공격력
    public int meleeAttackPower = 10;
    // 중거리 공격 범위
    public float shotAttackDistance = 7f;
    // 중거리 공격력
    public int shotAttackPower = 5;
    // 보스의 오른손
    public Transform rightHand;
    // 회전할것인가?
    bool isRoatate = false;
    // 회전속도
    public float rotationSpeed = 2f;
    // 회전 후 대기 시간
    public float pauseDuration = 1f;
    // 원래 회전각
    private Quaternion originalRotation;
    // 타겟 회전각
    private Quaternion targetRotation;
    // 플레이어가 가까운가?
    bool isPlayerClose = false;

    // 돌진 속도
    public float chargeSpeed = 10f;
    // 돌진 시작 거리
    public float chargeRange = 15f;
    // 돌진 여부
    public bool isCharging = false;
    // 캐릭터의 동작 여부
    bool isMoving = false;

    void Start()
    {
        // 최초의 보스 상태는 Idle
        state = EnemyState.Idle;
        // Player의 Transform 컴포넌트 받아오기
        player = GameObject.Find("Player").transform;
        // Boss의 캐릭터 컨트롤러 컴포넌트 받아오기
        cc = GetComponent<CharacterController>();
        // 초기 회전 상태 저장
        originalRotation = rightHand.rotation;
        targetRotation = Quaternion.Euler(-50, -47, 54) * originalRotation;
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
        // 플레이어와 보스의 거리 구하기
        float dist = Vector3.Distance(player.transform.position, transform.position);

        // 적이 플레이어와의 거리가 근접 공격 범위 이내에 있으면 공격 상태로 전환
        if (dist < meleeAttackDistance)
        {
            state = EnemyState.Attack;
            // 이동 벡터를 0으로 설정하여 이동을 멈춘다.
            cc.Move(Vector3.zero);
            return;
        }

        // 플레이어와의 거리가 공격 범위 밖이라면 플레이어 방향으로 향한다.
        dir = player.transform.position - transform.position;
        dir.Normalize();
        dir.y = 0;
        // 캐릭터 컨트롤러를 이용하여 이동
        cc.Move(dir * moveSpeed * Time.deltaTime);
    }

    // 공격 함수
    void Attack()
    {
        // 만약, 플레이어가 공격 범위 이내에 있다면 플레이어를 공격한다.
        float dist = Vector3.Distance(player.transform.position, transform.position);

        // 공격 패턴에 따른 공격 범위 및 처리
        // 근거리
        if(dist < meleeAttackDistance)
        {
            currTime += Time.deltaTime;
            if(currTime >= attackDelayTime)
            {
                MeleeAttack();
                currTime = 0;
            }
        }
        // 중거리
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
    // 근접 공격
    void MeleeAttack()
    {
        // 오른팔을 회전시킨다.
        isRoatate = true;
        rightHand.rotation = targetRotation;

        // 플레이어와 멀어지면
        if(isPlayerClose)
        {
            // 오른손을 원래 위치로 한다.
            rightHand.rotation = originalRotation;
        }

        // 오른쪽 팔의 드릴을 이용하여 후려친다. 패턴은 위 대각선, 아래 대각선 2개로 랜덤하게 부여
        print("근접 공격");
    }
    
    // 중거리 랜덤 공격
    void RandomShotAttack()
    {
        // 중거리 공격 2개중 랜덤하게 부여
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

    // 중거리 공격1 - 돌진 공격
    void ShotAttackType1()
    {
        // 플레이어와의 거리 
        float dist = Vector3.Distance(player.transform.position, transform.position);
        // 플레이어와의 거리가 돌진 범위 보다 크다면
        if (dist < chargeRange)
        {
            if (!isCharging)
            {
                print("돌진 공격");
                StartCoroutine(ChargeTowardsPlayer());
            }
        }
        // 돌진공격 , 전방위 공격 패턴 2개로 랜덤하게 부여

    }

    // 중거리 공격2 - 전방위 공격(땅내려치기)
    void ShotAttackType2()
    {
        print("땅내려치기");
    }

    // 플레이어를 향해 돌진, 일정 시간이 지나면 이동 상태로 돌아간다.
    private IEnumerator ChargeTowardsPlayer()
    {
        isCharging = true;
        this.moveSpeed = chargeSpeed;

        float chargeDutation = 1f;
        float startTime = Time.time;

        while(Time.time < startTime + chargeDutation)
        {
            print("돌진중");

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
