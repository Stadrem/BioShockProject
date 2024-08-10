//using System.Collections;
//using UnityEngine;
//using UnityEngine.AI;

//public class BossBehavior : MonoBehaviour
//{
//    // 파티클 시스템 오브젝트
//   GameObject particlesRing;

//    // 에너미 상태 열거형
//    public enum EnemyState
//    {
//        Idle,
//        Move,
//        Melee,
//        ShotAttackType1,
//        ShotAttackType2,
//        Damaged,
//        Die
//    }

//    // 에너미 상태 변수
//    public EnemyState state;

//    // 플레이어 Transform
//    private Transform player;

//    // 공격 지연시간, 현재 시간
//    private float currTime;
//    public float attackDelayTime = 2f;

//    // 보스 공격력 및 속도
//    public int attackPower = 3;
//    public float moveSpeed = 2f;

//    // 근접 공격 관련 변수
//    public float meleeAttackDistance = 4f;
//    public int meleeAttackPower = 10;

//    // 중거리 공격 관련 변수
//    public float shotAttackDistance = 20f;
//    public int shotAttackPower = 5;

//    // 회전 관련 변수
//    public float rotationSpeed = 2f;
//    private Quaternion targetRotation;

//    // 돌진 관련 변수
//    public float chargeSpeed = 10f;
//    public float chargeRange = 7f;
//    private bool isCharging = false;

//    // 넉백 관련 변수
//    public float knockbackDistance = 10f;
//    public float knockbackTime = 0.2f;
//    private bool isKnockback = false;

//    // NavMeshAgent, Animator
//    private NavMeshAgent agent;
//    private Animator anim;

//    // 초기화
//    void Start()
//    {
//        anim = GetComponentInChildren<Animator>();
//        state = EnemyState.Idle;
//        player = GameObject.Find("Player").transform;
//        agent = GetComponent<NavMeshAgent>();
//        currTime = 0;
//    }

//    void Update()
//    {
//        // 플레이어를 향한 회전
//        RotateTowardsPlayer();

//        // 넉백 처리
//        HandleKnockback();

//        // 현재 상태에 따른 행동 처리
//        switch (state)
//        {
//            case EnemyState.Idle:
//                HandleIdleState();
//                break;
//            case EnemyState.Move:
//                HandleMoveState();
//                break;
//            case EnemyState.Melee:
//                HandleMeleeAttack();
//                break;
//            case EnemyState.ShotAttackType1:
//                HandleShotAttackType1();
//                break;
//            case EnemyState.ShotAttackType2:
//                HandleShotAttackType2();
//                break;
//            case EnemyState.Damaged:
//                // Handle Damaged behavior if needed
//                break;
//            case EnemyState.Die:
//                HandleDieState();
//                break;
//        }
//    }

//    // 상태 변경 함수
//    public void ChangeState(EnemyState newState)
//    {
//        state = newState;
//        currTime = 0; // 상태 변경 시 시간 초기화

//        switch (state)
//        {
//            case EnemyState.Idle:
//                agent.isStopped = true;
//                anim.SetTrigger("Idle");
//                break;
//            case EnemyState.Move:
//                agent.isStopped = false;
//                anim.SetTrigger("Move");
//                break;
//            case EnemyState.Melee:
//                agent.isStopped = true;
//                anim.SetTrigger("Melee");
//                break;
//            case EnemyState.ShotAttackType1:
//                anim.SetTrigger("Shot");
//                break;
//            case EnemyState.ShotAttackType2:
//                agent.isStopped = true;
//                anim.SetTrigger("Shot2");
//                break;
//            case EnemyState.Damaged:
//                agent.isStopped = true;
//                anim.SetTrigger("Damage");
//                break;
//            case EnemyState.Die:
//                agent.isStopped = true;
//                anim.SetTrigger("Die");
//                StartCoroutine(RemoveAfterDelay(2.0f));
//                break;
//        }
//    }

//    // 플레이어를 향해 회전하는 함수
//    private void RotateTowardsPlayer()
//    {
//        Vector3 directionToPlayer = player.position - transform.position;
//        directionToPlayer.y = 0;

//        if (directionToPlayer != Vector3.zero)
//        {
//            targetRotation = Quaternion.LookRotation(directionToPlayer);
//            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
//        }
//    }

//    // 넉백 처리 함수
//    private void HandleKnockback()
//    {
//        if (isKnockback)
//        {
//            currTime += Time.deltaTime;
//            GameManager.instance.player.transform.position += Vector3.back * 50 * Time.deltaTime;

//            if (currTime > knockbackTime)
//            {
//                isKnockback = false;
//                currTime = 0;
//            }
//        }
//    }

//    // 대기 상태 처리 함수
//    private void HandleIdleState()
//    {
//        currTime += Time.deltaTime;
//        if (currTime >= 5f) // Idle 상태 지속 시간
//        {
//            ChangeState(EnemyState.Move);
//        }
//    }

//    // 이동 상태 처리 함수
//    private void HandleMoveState()
//    {
//        float dist = Vector3.Distance(player.position, transform.position);

//        if (dist <= meleeAttackDistance)
//        {
//            ChangeState(EnemyState.Melee);
//        }
//        else if (dist > 10f)
//        {
//            ChangeState(EnemyState.ShotAttackType1);
//        }
//        else if (dist > 5f)
//        {
//            ChangeState(EnemyState.ShotAttackType2);
//        }
//        else
//        {
//            agent.SetDestination(player.position);
//        }
//    }

//    // 근접 공격 처리 함수
//    private void HandleMeleeAttack()
//    {
//        currTime += Time.deltaTime;
//        if (currTime >= attackDelayTime)
//        {
//            isKnockback = true;
//            GameManager.instance.Damaged(meleeAttackPower);
//            currTime = 0;
//        }
//    }

//    // 중거리 공격1 (돌진 공격) 처리 함수
//    private void HandleShotAttackType1()
//    {
//        if (!isCharging)
//        {
//            StartCoroutine(ChargeTowardsPlayer());
//        }
//    }

//    // 돌진 코루틴
//    private IEnumerator ChargeTowardsPlayer()
//    {
//        isCharging = true;
//        float originalSpeed = agent.speed;
//        agent.speed = chargeSpeed;

//        // 플레이어와 보스의 거리가 10 이상인 경우에만 돌진 시작
//        if (Vector3.Distance(player.position, transform.position) > 10f)
//        {
//            agent.SetDestination(player.position);

//            // 계속해서 플레이어를 향해 이동하고, 거리가 10 미만이 되면 빠져나옴
//            while (Vector3.Distance(player.position, transform.position) > 10f)
//            {
//                yield return null;  // 매 프레임마다 거리 체크
//            }
//        }

//        agent.speed = originalSpeed;
//        isCharging = false;
//        ChangeState(EnemyState.Move);
//    }

//    // 중거리 공격2 (땅내려치기) 처리 함수
//    private void HandleShotAttackType2()
//    {
//        anim.SetTrigger("Shot2");
//        StartCoroutine(WaitAndChangeState(6.2f));
//    }

//    // 상태 변경 코루틴
//    private IEnumerator WaitAndChangeState(float waitTime)
//    {
//        yield return new WaitForSeconds(waitTime);
//        ChangeState(EnemyState.Move);
//    }

//    // 피격 처리 함수
//    public void Damaged(int damage, string type)
//    {
//        anim.SetTrigger("Damage");
//        GetComponent<BossDamaged>().Damaged(damage, type);
//    }

//    // 사망 처리 함수
//    private void HandleDieState()
//    {
//        anim.SetTrigger("Die");
//    }

//    // 오브젝트 제거 코루틴
//    private IEnumerator RemoveAfterDelay(float delay)
//    {
//        yield return new WaitForSeconds(delay);
//        Destroy(gameObject);
//    }

//    // 파티클 생성 함수
//    private void ParticleMake()
//    {
//        GameObject rising = Instantiate(particlesRing, transform.position, Quaternion.identity);
//        ParticleSystem ps = rising.GetComponent<ParticleSystem>();
//        if (ps != null)
//        {
//            ps.Play();
//        }
//        Destroy(rising, 2f);
//    }

//    // 충돌 처리 함수
//    private void OnTriggerEnter(Collider other)
//    {
//        if (state == EnemyState.Melee || state == EnemyState.ShotAttackType1)
//        {
//            ParticleMake();

//            if (other.CompareTag("Player"))
//            {
//                GameManager.instance.Damaged(attackPower);
//            }
//        }
//    }
//}