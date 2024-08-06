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
    public float shotAttackDistance = 8f;
    // 중거리 공격력
    public int shotAttackPower = 5;
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
    public float chargeSpeed = 20f;
    // 돌진 시작 거리
    public float chargeRange = 7f;
    // 돌진 여부
    public bool isCharging = false;
    // 캐릭터의 동작 여부
    bool isMoving = false;
    // 보스 데미지 스크립트 참조
    private BossDamaged bossDamaged;

    // 넉백 힘
    public float knockbackDistance = 10f;
    // 넉백 시간
    public float knockbackTime = 0.2f;

    private bool isKnockback = false;
    private Vector3 knockbackDirection;
    private float knockbackStartTime;
    public float knockbackDuration = 0.2f;
    // 충돌 감지 반경
    public float collisionRadius = 1f;

    // NavMeshAgent
    NavMeshAgent agent;
    // Animation Controller
    Animator ani;

    void Start()
    {
        // 최초의 보스 상태는 Idle
        state = EnemyState.Idle;
        // Player의 Transform 컴포넌트 받아오기
        player = GameObject.Find("Player").transform;
        // NavMeshAgent 컴포넌트
        agent = GetComponent<NavMeshAgent>();
        // 보스 데미지 스크립트
        bossDamaged = GetComponent<BossDamaged>();
        // 애니메이션 컨트롤러
        ani = GetComponent<Animator>();
    }
     
    void Update()
    {
        // 플레이어가 있는 방향으로 몸을 회전시킨다.
        Vector3 directionToPlayer = player.position - transform.position;
        directionToPlayer.y = 0;

        if(directionToPlayer != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
            //print(directionToPlayer);
            // 보간을 이용하여 속도 조절
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 1f);
        }
        
        // 넉백 처리
        if(isKnockback)
        {
            float knockbackProgress = (Time.time - knockbackStartTime) / knockbackDuration;
            if(knockbackProgress < 1f)
            {
                player.position += knockbackDirection * (knockbackDistance * Time.deltaTime / knockbackDuration);
            }
            else
            {
                isKnockback = false;
            }
        }

        switch (state)
        {
            case EnemyState.Idle:
                //ani.SetTrigger("Idle");
                Idle();
                break;
            case EnemyState.Move:
                //ani.SetTrigger("Move");
                Move();
                break;
            case EnemyState.Attack:
                Attack();
                break;
            case EnemyState.Damaged:
                // Damaged 상태에서 특정 행동을 취할 수 있다.
                break;
            case EnemyState.Die:
                Die();
                break;
        }
    }

    // 상태 변경 함수
    public void ChangeState(EnemyState newState)
    {
        state = newState;
        //UpdateAnimator();

        switch(state)
        {
            case EnemyState.Idle:
                agent.isStopped = true;
                // 대기 상태 로직
                break;
            case EnemyState.Move:
                agent.isStopped = false;
                // 이동 상태 로직
                break;
            case EnemyState.Attack:
                agent.isStopped = true;
                // 공격 상태 로직
                break;
            case EnemyState.Damaged:
                agent.isStopped = true;
                // 피해 상태 로직
                break;
            case EnemyState.Die:
                agent.isStopped = true;
                // 2초 후에 오브젝트를 제거시킨다.
                StartCoroutine(RemoveAfterDelay(2.0f));
                // 죽음 상태 로직
                break;
        }
    }
    //void UpdateAnimator()
    //{
    //    if (ani != null)
    //    {
    //        ani.SetTrigger("Idle");
    //        ani.SetTrigger("Move");
    //        //ani.SetTrigger("Attack");
    //        //ani.SetTrigger("Damaged");
    //        //ani.SetTrigger("Die");
    //    }
    //}

    // 대기 상태 함수
    public void Idle()
    {
        // 플레이어와의 거리가 인지범위 안에 들어오면, Move 상태로 전환한다.
        float dist = Vector3.Distance(player.transform.position, transform.position);
        if (findDistance > dist)
        {
            // move 상태로 변환
            ChangeState(EnemyState.Move);
            //state = EnemyState.Move;
        }
    }

    // 이동 상태 함수
    public void Move()
    {
        // 플레이어와 보스의 거리 구하기
        float dist = Vector3.Distance(player.transform.position, transform.position);

        // 적이 플레이어와의 거리가 근접 공격 범위 이내에 있으면 공격 상태로 전환
        if (dist < meleeAttackDistance || dist < shotAttackDistance)
        {
            ChangeState(EnemyState.Attack);
            return;
        }
        // 적이 플레이어와의 거리가 중거리 공격 범위 이내에 있으면 공격 상태로 전환
        //if (dist > shotAttackDistance)
        //{
        //    ChangeState(EnemyState.Attack);
        //}

        // navmeshagent를 이용하여 플레이어 방향으로 이동한다.
        agent.SetDestination(player.position);
    }

    // 공격 함수
    // 플레이어가 공격 받았을 때 뒤로 밀리는 효과 추가(플레이어 위치 받아와서 back으로)
    public void Attack()
    {
        // 만약, 플레이어가 공격 범위 이내에 있다면 플레이어를 공격한다.
        float dist = Vector3.Distance(player.transform.position, transform.position);

        // 공격 패턴에 따른 공격 범위 및 처리
        // 근거리
        if (dist < meleeAttackDistance)
        {
            currTime += Time.deltaTime;
            if (currTime >= attackDelayTime)
            {
                // 싱글톤으로 HP 관리
                GameManager.instance.Damaged(attackPower);

                // 플레이어에게 넉백 적용
                ApplyKnockback(player.position - transform.position);

                MeleeAttack();
                currTime = 0;
            }
        }
        // 중거리
        else if (dist < shotAttackDistance)
        {
            currTime += Time.deltaTime;
            if (currTime >= attackDelayTime)
            {
                // 싱글톤으로 HP 관리
                GameManager.instance.Damaged(attackPower);

                // 플레이어에게 넉백 적용
                ApplyKnockback(player.position - transform.position);

                RandomShotAttack();
                currTime = 0;
            }
        }
        else
        {
            ChangeState(EnemyState.Move);
        }
    }
    // 근접 공격
    public void MeleeAttack()
    {
        print("근접 공격");
    }

    // 중거리 랜덤 공격
    public void RandomShotAttack()
    {
        // 중거리 공격 2개중 랜덤하게 부여
        int attackType = Random.Range(0, 2);

        if (attackType == 0)
        {
            // 중거리 공격1
            ShotAttackType1();
        }
        else
        {
            // 중거리 공격2
            ShotAttackType2();
        }
    }

    // 중거리 공격1 - 돌진 공격
    public void ShotAttackType1()
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
    }

    // 중거리 공격2 - 전방위 공격(땅내려치기)
    public void ShotAttackType2()
    {
        // 카메라 시야 흐려짐
        print("땅내려치기");
    }

    // 플레이어를 향해 돌진, 일정 시간이 지나면 이동 상태로 돌아간다.
    private IEnumerator ChargeTowardsPlayer()
    {
        // 돌진할것인가
        isCharging = true;
        // 이동속도를 돌진속도로 변환
        float originMoveSpeed = agent.speed;
        agent.speed = chargeSpeed;

        float chargeDuration = 2f;
        float startTime = Time.time;

        // 지정된 시간동안 돌진
        while (Time.time < startTime + chargeDuration)
        {
            agent.SetDestination(player.position);
            print("돌진중");
            yield return null;
        }
        agent.speed = originMoveSpeed;
        isCharging = false;
        // ChangeState?
        ChangeState(EnemyState.Move);
    }

    public void Damaged(int damage, string type)
    {
        GetComponent<BossDamaged>().Damaged(damage, type);
    }

    public void Die()
    {
        print("사망");
    }

    private IEnumerator RemoveAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    // 공격시 충돌 처리 
    private void OnTriggerEnter(Collider other)
    {
        // 객체가 Player, 에너미 상태가 attack일 경우
        if(other.CompareTag("Player") && state == EnemyState.Attack)
        {
            // 플레이어에게 피해를 입힌다.
            GameManager.instance.Damaged(attackPower);
        }
    }

    // 넉백 효과 적용 메서드
    private void ApplyKnockback(Vector3 direction)
    {
        direction.y = 0;

        knockbackDirection = direction.normalized;
        print(knockbackDirection);
        knockbackStartTime = Time.time;
        isKnockback = true;       
    }
}
