using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossBehavior : MonoBehaviour
{
    // 파티클 시스템 오브젝트
    public GameObject particlesRing;
    // 에너미 상태
    public enum EnemyState
    {
        Idle,
        Move,
        Attack,
        Melee,
        ShotAttack,
        Damaged,
        Die
    }
    // 에너미 상태 변수
    public EnemyState state;
    // 플레이어 공격 가능 범위
    //public float attackDistance = 2f;
    // Player Transform
    Transform player;
    // 현재 시간
    float currTime = 0;
    // 공격 딜레이 시간
    float attackDelayTime = 2f;
    // 이동 방향 
    //Vector3 dir;
    // 보스 공격력
    public int attackPower = 3;
    // 이동속도
    public float moveSpeed = 2;

    // 근접 공격 범위
    public float meleeAttackDistance = 4f;
    // 근접 공격력
    public int meleeAttackPower = 10;
    // 중거리 공격 범위
    public float shotAttackDistance = 20f;
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
    public float chargeSpeed = 10f;
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
    // 넉백 되었는가?
    private bool isKnockback = false;
    private Vector3 knockbackDirection;
    private float knockbackStartTime;
    public float knockbackDuration = 0.2f;
    // 충돌 감지 반경
    public float collisionRadius = 1f;

    // NavMeshAgent
    NavMeshAgent agent;
    // Animation Controller
    Animator anim;

    void Start()
    {
        // 애니메이션 컨트롤러
        anim = GetComponentInChildren<Animator>();
        // 최초의 보스 상태는 Idle
        state = EnemyState.Idle;
        // Player의 Transform 컴포넌트 받아오기
        player = GameObject.Find("Player").transform;
        // NavMeshAgent 컴포넌트
        agent = GetComponent<NavMeshAgent>();
        // 보스 데미지 스크립트
        bossDamaged = GetComponent<BossDamaged>();

    }

    void Update()
    {
        // 플레이어가 있는 방향으로 몸을 회전시킨다.
        Vector3 directionToPlayer = player.position - transform.position;
        directionToPlayer.y = 0;

        if (directionToPlayer != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
            //print(directionToPlayer);
            // 보간을 이용하여 속도 조절
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 1f);
        }

        // 넉백 처리
        if (isKnockback == true)
        {
            currTime += Time.deltaTime;
            GameManager.instance.player.transform.position += Vector3.back * 50 * Time.deltaTime;

            if (currTime > knockbackTime)
            {
                isKnockback = false;
                currTime = 0;
            }
        }

        //if (isKnockback)
        //{
        //    float knockbackProgress = (Time.time - knockbackStartTime) / knockbackDuration;
        //    if (knockbackProgress < 1f)
        //    {
        //        Vector3 knockbackMovement = knockbackDirection * (knockbackDistance * Time.deltaTime / knockbackDuration);
        //        //agent.Move(knockbackMovement);

        //    }
        //    else
        //    {
        //        isKnockback = false;
        //    }
        //}

        switch (state)
        {
            case EnemyState.Idle:
                Idle();
                //anim.SetTrigger("Idle");
                break;
            case EnemyState.Move:
                Move();
                break;
            case EnemyState.Melee:
                MeleeAttack();
                break;
            case EnemyState.ShotAttack:
                //ShotAttack();
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
        switch (state)
        {
            case EnemyState.Idle:
                agent.isStopped = true;
                // 대기 상태 애니메이션
                anim.SetTrigger("Idle");
                break;
            case EnemyState.Move:
                agent.isStopped = false;
                // 이동 상태 애니메이션
                anim.SetTrigger("Move");
                break;
            case EnemyState.Melee:
                agent.isStopped = true;
                //isKnockback = true;
                // 애니메이션
                anim.SetTrigger("Melee");
                break;
            case EnemyState.ShotAttack:
                {
                    // 공격 상태 로직
                    int attackType = Random.Range(0, 2);
                    if (attackType == 0)
                    {
                        // 중거리 공격1
                        ShotAttackType1();
                    }
                    else
                    {
                        agent.isStopped = true;
                        // 중거리 공격2
                        ShotAttackType2();
                    }
                }
                break;
            case EnemyState.Damaged:
                agent.isStopped = true;
                anim.SetTrigger("Damage");
                break;
            case EnemyState.Die:
                {
                    agent.isStopped = true;
                    // 2초 후에 오브젝트를 제거시킨다.
                    StartCoroutine(RemoveAfterDelay(2.0f));
                    anim.SetTrigger("Die");
                }
                break;
        }
    }

    // 대기 시간
    public float idleTIme = 5f;
    // 대기 상태 함수
    public void Idle()
    {
        //anim.SetTrigger("Idle");

        currTime += Time.deltaTime;
        if (idleTIme <= currTime)
        {
            currTime = 0;

            ChangeState(EnemyState.Move);
        }
    }
    // 이동 상태 함수
    public void Move()
    {
        // 플레이어와 보스의 거리 구하기
        float dist = Vector3.Distance(player.transform.position, transform.position);

        // 플레이어와의 거리가 근거리 공격 범위안이라면
        if (dist <= meleeAttackDistance)
        {
            // 근거리로 상태 전환
            ChangeState(EnemyState.Melee);
        }
        // 아니면
        else if (dist <= shotAttackDistance)
        {
            // 중거리로 상태 전환
            ChangeState(EnemyState.ShotAttack);
        }
        else
        {
            agent.SetDestination(player.position);
            //anim.SetTrigger("Move");
        }
    }

    // 근접 공격
    public void MeleeAttack()
    {
        print("근접 공격");
        ParticleMake();

        // 시간을 흐르게 하자
        currTime += Time.deltaTime;
        // 공격 지연시간 경과시
        if (currTime >= attackDelayTime)
        {
  
            // 플레이어에게 넉백 적용
            isKnockback = true;
            //ApplyKnockback(player.position - transform.position);
            // 싱글톤으로 HP 관리
            GameManager.instance.Damaged(attackPower);
            // 초기화
            currTime = 0;
        }
        // 플레이어와의 거리 다시 계산
        float dist = Vector3.Distance(player.position, transform.position);
        // 만약 플레이어와의 거리가 근접 공격 가능 범위에서 벗어나면
        if (dist > meleeAttackDistance)
        {
            // 중거리 공격 상태로 전환
            ChangeState(EnemyState.ShotAttack);
        }
    }

    // 공격 하고있는가?
    bool isAttacking;

    // 중거리 공격 함수
    public void ShotAttack()
    {
        // 시간을 흐르게 하자.

        if (isAttacking == false)
        {
            currTime += Time.deltaTime;

        }
        if (currTime >= attackDelayTime)
        {
            isAttacking = true;
            // 공격 패턴에 따른 공격 범위 및 처리
            // 중거리 패턴 1 / 2
            // 중거리 공격 2개중 랜덤하게 부여
            //int attackType = 0;// Random.Range(0, 2);
            //if (attackType == 0)
            //{
            //    // 중거리 공격1
            //    ShotAttackType1();
            //}
            //else
            //{
            //    // 중거리 공격2
            //    ShotAttackType2();
            //}
            // 싱글톤으로 HP 관리
            GameManager.instance.Damaged(attackPower);
            // 초기화
            currTime = 0;
        }

        // 플레이어와의 거리 다시 계산
        float dist = Vector3.Distance(player.position, transform.position);
        // 중거리 공격 범위 밖이면
        if (dist > shotAttackDistance)
        {
            // 이동 상태로 전환
            ChangeState(EnemyState.Move);
        }
        // 근거리 범위로 들어왔다면
        else if (dist <= meleeAttackDistance)
        {
            // 근거리 공격 전환
            ChangeState(EnemyState.Melee);
        }
    }

    // 중거리 공격1 - 돌진 공격
    public void ShotAttackType1()
    {
        anim.SetTrigger("Shot");

        StartCoroutine(ChargeTowardsPlayer());
    }

    // 중거리 공격2 - 전방위 공격(땅내려치기)
    public void ShotAttackType2()
    {
        anim.SetTrigger("Shot2");
        print("땅내려치기");

        // 일정시간이 지난 후 상태를 변경, 빠져나온다
        StartCoroutine(WaitAndChageState(6.2f));
    }

    // 상태바꾸기 코루틴 함수
    private IEnumerator WaitAndChageState(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        isAttacking = false;
        ChangeState(EnemyState.Move);
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
        // 속도 원래 속도로 바꾸기
        agent.speed = originMoveSpeed;
        // 돌진 끝
        isCharging = false;
        // 공격 끝
        isAttacking = false;

        ChangeState(EnemyState.Move);
        //agent.SetDestination(player.position);
    }

    public void Damaged(int damage, string type)
    {
        anim.SetTrigger("Damage");

        GetComponent<BossDamaged>().Damaged(damage, type);

    }

    public void Die()
    {
        anim.SetTrigger("Die");
        print("사망");

    }

    private IEnumerator RemoveAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    // 파티클 라이징 생성 함수
    void ParticleMake()
    {
        // 파티클 라이징 생성시킨다.
        GameObject rising = Instantiate(particlesRing);
        // 파티클 라이징의 위치를 빅대디의 위치로 한다.
        rising.transform.position = transform.position;
        // 파티클 시스템 컴포넌트 가져오기
        ParticleSystem ps = rising.GetComponent<ParticleSystem>();
        // 컴포넌트 있으면 실행하게 하기
        if (ps != null)
        {
            ps.Play();
        }
        // 2초가 지나면 파괴하게 하기
        Destroy(rising, 2);
    }

    // 공격시 충돌 처리 
    private void OnTriggerEnter(Collider other)
    {

        print(state);

        // 공격 상태일 때 파티클 생성
        if(state == EnemyState.Melee || state == EnemyState.ShotAttack)
        {
            // 부딪히면 파티클 생성
            ParticleMake();

            // 맞은 대상이 플레이어라면
            if(other.CompareTag("Player"))
            {
                print("피해입히기");
                // 플레이어에게 피해를 입힌다.
                GameManager.instance.Damaged(attackPower);
            }  
        }
    }

    // 넉백 효과 적용 메서드
    //private void ApplyKnockback(Vector3 direction)
    //{
    //    direction.y = 0;

    //    knockbackDirection = direction.normalized;
    //    print(knockbackDirection);
    //    knockbackStartTime = Time.time;
    //    isKnockback = true;
    //}
}
