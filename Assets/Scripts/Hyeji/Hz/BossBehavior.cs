using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossBehavior : MonoBehaviour
{
    // 파티클 시스템 오브젝트
    GameObject particlesRing;
    // 에너미 상태
    public enum EnemyState
    {
        Idle,
        Move,
        Attack,
        Melee,
        ShotAttack,
        //ShotAttackType1,
        //ShotAttackType2,
        Damaged,
        Die
    }
    // 에너미 상태 변수
    public EnemyState state;
    // Player Transform
    public Transform player;
    // 현재 시간
    public float currTime = 0;
    // 공격 딜레이 시간
    public float attackDelayTime = 2f;
    // 이동 방향 
    //Vector3 dir;
    // 보스 공격력
    public int attackPower = 3;
    // 이동속도
    public float moveSpeed = 2;

    // 근접 공격 범위
    public float meleeAttackDistance = 10f;
    // 근접 공격력
    public int meleeAttackPower = 10;
    // 중거리 공격 범위
    public float shotAttackDistance = 20f;
    // 중거리 공격력
    public int shotAttackPower = 5;
    // 회전할것인가?
    public bool isRoatate = false;
    // 회전속도
    public float rotationSpeed = 2f;
    // 회전 후 대기 시간
    public float pauseDuration = 1f;
    // 원래 회전각
    public Quaternion originalRotation;
    // 타겟 회전각
    public Quaternion targetRotation;
    // 플레이어가 가까운가?
    public bool isPlayerClose = false;

    // 돌진 속도
    public float chargeSpeed = 10f;
    // 돌진 시작 거리
    public float chargeRange = 7f;
    // 돌진 여부
    public bool isCharging = false;
    // 캐릭터의 동작 여부
    public bool isMoving = false;
    // 보스 데미지 스크립트 참조
    private BossDamaged bossDamaged;

    // 넉백 힘
    public float knockbackDistance = 10f;
    // 넉백 시간
    public float knockbackTime = 0.2f;
    // 넉백 되었는가?
    public bool isKnockback = false;
    public Vector3 knockbackDirection;
    public float knockbackStartTime;
    public float knockbackDuration = 0.2f;
    // 충돌 감지 반경
    public float collisionRadius = 1f;

    // NavMeshAgent
    NavMeshAgent agent;
    // Animation Controller
    Animator anim;

    // 보스행동 스크립트
    private OnMeleeAttackEnd onMelee;

    // 플레이어 위치를 저장하는 큐
    private Queue<Vector3> playerPositions = new Queue<Vector3>();
    // 플레이어의 위치를 저장하는 간격
    public float recordInterval = 0.1f;
    // 0.2초 전 플레이어 위치를 위한 타이머
    private float recordTimer = 0f;
    // 저장할 최대 위치 수 (0.2초 동안 저장할 위치의 수)
    private int maxRecordedPositions = 2;
    // 인식 거리 (플레이어가 이 거리 내로 들어오면 보스가 추적을 시작함)
    public float detectionRange = 15f;

    public float meleeAttackRange = 5f; // 근접 공격 거리
    public LayerMask playerLayer; // 플레이어가 속한 레이어
    public Transform rayOrigin; // 레이 오브젝트



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
        // bossBehavior 스크립트 참조
        onMelee = GetComponent<OnMeleeAttackEnd>();

        StartCoroutine(RecordPlayerPosition());

    }

    void Update()
    {
        // 플레이어와의 거리 계산
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);

        // 보스가 플레이어와 너무 가까이 붙지 않도록 거리 유지
        //if (distanceToPlayer < 2.9f)  // 예시: 최소 2미터 거리를 유지
        //{
        //    Vector3 direction = (transform.position - player.transform.position).normalized;
        //    transform.position = player.transform.position + direction * 2.9f;
        //}


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

        switch (state)
        {
            case EnemyState.Idle:
                Idle(distanceToPlayer);
                break;
            case EnemyState.Move:
                Move();
                break;
            case EnemyState.Melee:
                MeleeAttack();
                break;
            case EnemyState.ShotAttack:
                ShotAttack();
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
        // 상태 변경 전에 플래그를 초기화합니다.
        if (newState != EnemyState.ShotAttack)
        {
            isInShotAttack = false;
            isAttacking = false; // ShotAttack 상태가 아닐 때 공격 플래그 초기화
        }

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
            //case EnemyState.ShotAttackType1:
            //    agent.isStopped = true;
            //    isKnockback = true;
            //    //anim.SetTrigger("Shot");
            //    break;
            //case EnemyState.ShotAttackType2:
            //    agent.isStopped = true;
            //    //anim.SetTrigger("Shot2");
            //    break;
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
    // public float idleTIme = 5f;
    // 대기 상태 함수
    public void Idle(float distanceToPlayer)
    {
        // 플레이어가 인식 범위 내로 들어왔을 때 추적 시작
        if (distanceToPlayer <= detectionRange)
        {
            ChangeState(EnemyState.Move);
            return;
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
        //ParticleMake();

        // 시간을 흐르게 하자
        currTime += Time.deltaTime;
        // 공격 지연시간 경과시
        if (currTime >= attackDelayTime)
        {
            print("근접 공격");
            // Ray 적용
            MeleeRay();

            // 상태를 전환하거나, 루프가 다시 돌아오지 않도록 설정
            ChangeState(EnemyState.Idle);

            // 초기화
            currTime = 0;
        }

        // 플레이어와 보스의 거리 구하기
        float dist = Vector3.Distance(player.transform.position, transform.position);
        if (dist > meleeAttackDistance)
        {
            
            ChangeState(EnemyState.ShotAttack);
            //ChangeState(EnemyState.Move);
        }
    }
 

    // 공격 하고있는가?
    bool isAttacking;
    // 이미 shotAttack?
    bool isInShotAttack = false;

    // 중거리 공격 함수
    public void ShotAttack()
    {
        if (isInShotAttack) return;

        isInShotAttack = true;

        // 시간을 흐르게 하자.
        if (isAttacking == false)
        {
            currTime += Time.deltaTime;

        }
        if (currTime >= attackDelayTime)
        {
            isAttacking = true;
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

    // 플레이어 위치를 기록하는 코루틴
    private IEnumerator RecordPlayerPosition()
    {
        while (true)
        {
            yield return new WaitForSeconds(recordInterval);
            if (playerPositions.Count >= maxRecordedPositions)
            {
                playerPositions.Dequeue(); // 가장 오래된 위치를 제거
            }
            playerPositions.Enqueue(player.position); // 현재 플레이어 위치를 큐에 추가
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
        anim.ResetTrigger("Shot");
        anim.ResetTrigger("Shot2");
        anim.ResetTrigger("Move");

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

        // 움직임으로 전환
        ChangeState(EnemyState.Move);
        
    }

    // 플레이어를 향해 돌진, 일정 시간이 지나면 이동 상태로 돌아간다.
    private IEnumerator ChargeTowardsPlayer()
    {
        // 돌진할것인가
        isCharging = true;

        // 큐에서 0.2초 전에 있었던 위치를 가져옵니다.
        Vector3 targetPosition;
        if (playerPositions.Count > 0)
        {
            targetPosition = playerPositions.Peek();
        }
        else
        {
            targetPosition = player.position; // 큐가 비어있으면 현재 위치로 대체
        }

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

    void MeleeRay()
    {
        Debug.Log("MeleeRay 호출됨");

        // 플레이어의 중심을 향하는 방향 벡터를 계산
        Vector3 origin = rayOrigin.position;
        Vector3 direction = (player.position - origin).normalized;

        // 현재 보스와 플레이어의 거리 확인
        float distanceToPlayer = Vector3.Distance(player.position, origin);
        Debug.Log("보스와 플레이어 간 거리: " + distanceToPlayer);

        // 레이캐스트 발사
        RaycastHit hit;

        Debug.DrawRay(origin, direction * meleeAttackDistance, Color.red, 1.0f);

        if (Physics.Raycast(origin, direction, out hit, meleeAttackDistance, playerLayer))
        {
            Debug.Log("RayCast 충돌 발생");

            if (hit.transform.CompareTag("Player"))
            {
                Debug.Log("플레이어와 충돌");
                isKnockback = true;
                GameManager.instance.Damaged(meleeAttackPower);
            }
        }
        else
        {
            Debug.Log("충돌없어");
        }


        //Debug.Log("MeleeRay 호출됨");
        //Vector3 direction = player.position - transform.position;

        //// 레이캐스트 발사
        //RaycastHit hit;
        //// 눈으로 확인해보기
        //Debug.DrawRay(transform.position, direction * meleeAttackRange, Color.red, 1.0f);

        //if (Physics.Raycast(transform.position, direction, out hit, meleeAttackRange, playerLayer))
        //{
        //    Debug.Log("???");
        //    Debug.Log(hit.transform.name);

        //    // 레이캐스트가 맞은 객체의 태그가 "Player"인지 확인
        //    if (hit.transform.CompareTag("Player"))
        //    {
        //        // 플레이어에게 피해를 입힌다.
        //        Debug.Log("Ray");
        //        // 플레이어에게 넉백 적용
        //        isKnockback = true;
        //        GameManager.instance.Damaged(meleeAttackPower);
        //    }
        //}
        //else
        //{
        //    Debug.Log("충돌없어");
        //}
    }

    // 공격시 충돌 처리 
    private void OnTriggerEnter(Collider other)
    {
        print(state);

        // 공격 상태일 때 파티클 생성
        if (state == EnemyState.ShotAttack)
        {
            // 부딪히면 파티클 생성
            //ParticleMake();

            // 맞은 대상이 플레이어라면
            if (other.CompareTag("Player"))
            {
                print("피해입히기");
                // 플레이어에게 피해를 입힌다.
                GameManager.instance.Damaged(attackPower);
            }
        }
    }
}
