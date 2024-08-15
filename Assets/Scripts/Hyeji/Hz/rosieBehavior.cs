using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class rosieBehavior : MonoBehaviour
{
    public enum EnemyState
    {
        Idle,
        Move,
        Attack,
        Damaged,
        Die
    }


    // 에너미의 상태 변수
    public EnemyState state;
    // Player 의 Transform
    Transform player;
    // NavMeshAgent
    private NavMeshAgent agent;
    // Animator
    Animator anim;

    // LineRenderer
    LineRenderer lr;
    public float lineDuration = 0.2f;

    // 거리 및 공격 회전 
    public float findDistance = 10f;
    public float attackDistance = 20f;
    public float attackDelayTime = 3f;
    public float currTime;
    public int attackPower = 2;
    public float rotationSpeed = 2f;

    public GameObject firePos;

    // 파티클 시스템 오브젝트 (총)
    public GameObject bulletLightFactory;

    // Audio Source
    private AudioSource audioSource;
    // 사운드 - 대기 상태
    public AudioClip IdleSound;
    // 사운드 - 이동 상태
    public AudioClip MoveSound;
    // 사운드 - 공격 상태
    public AudioClip AttackSound;
    // 사운드 - 빅대디 데미지 상태
    public AudioClip damageSound;
    // 사운드 - 빅대디 죽음 상태
    public AudioClip dieSound;

    bool isDie = false;
    public int angry;

    // bool 값 변수
    DieScript dieScript;

    BossDamaged bossDamaged;

    public void ChangeState(BossBehavior.EnemyState s)
    {
        EnemyState _state = (EnemyState)s;
        ChangeState(_state);
    }

    // Start is called before the first frame update
    void Start()
    {
        // Animator
        anim = GetComponentInChildren<Animator>();
        // 최초의 보스 상태 Idle
        state = EnemyState.Idle;
        // 플레이어의 Transform 찾자
        player = GameObject.Find("Player").transform;
        // NavMeshAgent 컴포넌트
        agent = GetComponent<NavMeshAgent>();
        // LineRenderer
        lr = GetComponent<LineRenderer>();
        // Audio
        audioSource = GetComponent<AudioSource>();

        // LineRenderer 
        lr.positionCount = 2;
        lr.enabled = false;

        // DieScript 참조
        dieScript = GetComponent<DieScript>();

        bossDamaged = GetComponent<BossDamaged>();
        bossDamaged.onChangeState = ChangeState;
    }

    void Update()
    {
        if (angry >= 5)
        {
            // 피격 횟수에 따른 애니메이션 딜레이 적용
            currTime += Time.deltaTime;
            // 리셋시간보다 현재시간이 커지면
            if (currTime > 7)
            {
                // 피격 횟수 초기화
                angry = 0;
                // 현재 시간 초기화
                currTime = 0;
            }
        }

        // 죽음 상태라면 빠져나가기
        if (state == EnemyState.Die)
        {
            return;
        }

        

        // 플레이어와의 거리 계산
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);

        switch (state)
        {
            case EnemyState.Idle:
                if(dieScript.die == false)
                {
                    Idle(distanceToPlayer);
                }
                break;
            case EnemyState.Move:
                if (dieScript.die == false)
                {
                    Move();
                }
                //anim.SetTrigger("WALK");
                break;
            case EnemyState.Attack:
                if (dieScript.die == false)
                {
                    Attack();
                }
                break;
            case EnemyState.Damaged:
                // Damaged 상태에서 특정 행동을 취할 수 있다.
                break;
            case EnemyState.Die:
                Die();
                break;
        }
    }

    private void ChangeState(EnemyState newState)
    {

        if (state == newState) return;  // 동일한 상태로의 전환을 막음

        if (isDie)
        {
            return;
        }

        Debug.Log("Changing state from " + state + " to " + newState);

        state = newState;

        switch (newState)
        {
            case EnemyState.Idle:
                agent.isStopped = true;
                anim.SetTrigger("IDLE");
                break;
            case EnemyState.Move:
                agent.isStopped = false;
                anim.SetTrigger("WALK");
                break;
            case EnemyState.Attack:
                // 공격 시에는 navmesh 멈추자
                //agent.isStopped = false;
                anim.SetTrigger("ATTACK");
                // 소리한번 내고
                if (AttackSound != null && audioSource != null)
                {
                    audioSource.PlayOneShot(AttackSound);
                    Debug.Log("AttackSound");
                }
                break;
            case EnemyState.Damaged:
                agent.isStopped = true;
                if (angry <= 5)
                {
                    anim.SetTrigger("DAMAGED");
                }
                // 피격 증가 및 초기화
                angry++;
                print("앵그리확인");
                break;
            case EnemyState.Die:
                anim.SetTrigger("DIE");
                dieScript.die = true;
                {
                    isDie = true;
                }
                
                break;
        }
    }

    // 인식 거리 (플레이어가 이 거리 내로 들어오면 보스가 추적을 시작함)
    public float detectionRange = 15f;
    void Idle(float distanceToPlayer)
    {
        //// 플레이어와의 거리 계산
        //float distanceToPlayer = Vector3.Distance(player.position, transform.position);

        //Debug.Log("distanceToPlayer: " + distanceToPlayer);
        //Debug.Log("detectionRange: " + detectionRange);

        // 플레이어가 인식 범위 내로 들어왔을 때 추적 시작
        if (distanceToPlayer <= detectionRange)
        {
            // 소리한번 내고
            if (IdleSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(IdleSound);
                Debug.Log("대기상태소리임");
            }

            ChangeState(EnemyState.Move);
            return;
        }
    }

    // 이동 상태 함수
    void Move()
    {
        print("움직이니?");

        float distanceToPlayer = Vector3.Distance(player.position, transform.position);

        if (distanceToPlayer <= attackDistance)
        {
            ChangeState(EnemyState.Attack);
        }
        else
        {
            // 소리 중복 재생 방지
            if (!audioSource.isPlaying && MoveSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(MoveSound);
                Debug.Log("Move 소리 재생");
            }

            agent.SetDestination(player.position); // 플레이어를 쫓아감

            // 애니메이션 중복 트리거 방지
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("WALK"))
            {
                anim.SetTrigger("WALK");
                Debug.Log("WALK 애니메이션 트리거");
            }
        }

        //else
        //{
        //    // 소리한번 내고
        //    if (MoveSound != null && audioSource != null)
        //    {
        //        audioSource.PlayOneShot(MoveSound);
        //        Debug.Log("Move");
        //    }

        //    agent.SetDestination(player.position); // 플레이어를 쫓아감
        //    anim.SetTrigger("WALK");
        //}
    }

    // 공격 상태 함수
    public void Attack()
    {
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);

        if (distanceToPlayer > attackDistance)
        {
            ChangeState(EnemyState.Move); // 공격 범위에서 벗어나면 다시 추적
            return;
        }

        // 플레이어를 향해 회전
        Vector3 directionToPlayer = player.position - transform.position;
        directionToPlayer.y = 0; // 수직 회전 방지
        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

        // 공격 타이머 증가
        currTime += Time.deltaTime;

        if (currTime >= attackDelayTime)
        {
            // NavMesh 멈추고 공격하라.
            agent.isStopped = true;
            print("공격!");
            // 파티클
            //Particle();
            // 공격 수행
            AttackRay(player.position);

            currTime = 0; // 공격 타이머 초기화
        }


        ///// 응ㅇㅇㅇㅇ

        //// 플레이어를 바라보기
        //Vector3 directionToPlayer = player.position - transform.position;
        //Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
        //transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 3f);

        //// 보스와 플레이어 거리
        //float dist = Vector3.Distance(player.transform.position, transform.position);     
        //if(dist < attackDistance)
        //{
        //    currTime += Time.deltaTime;
        //    // 공격 지연 시간이 경과했을 때
        //    if (currTime >= attackDelayTime)
        //    {
        //        // NavMesh 멈추고 공격하라.
        //        agent.isStopped = true;
        //        print("공격!");

        //        // Raycast를 이용한 공격 패턴(원거리)
        //        AttackRay(player.transform.position);

        //        // 싱글톤으로 HP 관리
        //        GameManager.instance.Damaged(attackPower);

        //        // 현재 시간을 초기화 해준다
        //        currTime = 0;
        //    }
        //    else
        //    {
        //        // 공격 중에도 플레이어를 추적하자
        //        if (!agent.isStopped)
        //        {
        //            agent.SetDestination(player.position);
        //        }
        //    }
        //}

    }

    public void AttackRay(Vector3 aimPos)
    {
        // 플레이어가 있는 방향으로 ray를 발사한다.
        Ray ray = new Ray(firePos.transform.position, aimPos - firePos.transform.position);
        RaycastHit hit;

        // 파티클
        Particle();

        // Raycast 거리 설정
        float rayDistance = 20f;
        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            // 라인 렌더러로 궤적 표시
            StartCoroutine(ShowBulletTrajectory(firePos.transform.position, hit.point));

            // 맞은 대상이 Player라면
            if (hit.collider.CompareTag("Player"))
            {
                // Player 에게 데미지를 주자
                GameManager.instance.Damaged(attackPower);
            }
        }
    }

    void Particle()
    {
        // 파티클 라이징 생성시킨다.
        GameObject bulletLight = Instantiate(bulletLightFactory);
        // 파티클 라이징의 위치를 빅대디의 위치로 한다.
        bulletLight.transform.position = firePos.transform.position;
        print("파티클나오나");
        // 파티클 시스템 컴포넌트 가져오기
        ParticleSystem ps = bulletLight.GetComponent<ParticleSystem>();
        // 컴포넌트 있으면 실행하게 하기
        if (ps != null)
        {
            ps.Play();
        }
        // 2초가 지나면 파괴하게 하기
        Destroy(bulletLight, 2);
    }

    // 라인 렌더러로 레이캐스트 궤적을 표시
    IEnumerator ShowBulletTrajectory(Vector3 start, Vector3 end)
    {
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        lr.enabled = true;

        yield return new WaitForSeconds(lineDuration); // 궤적을 일정 시간 동안 표시

        lr.enabled = false;
    }

    // 데미지 상태 함수
    void Damaged(int damage, string type)
    {
        // 소리한번 내고
        if (damageSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(damageSound);
            Debug.Log("Damage Sound");
        }

        // 보스 데미지드 함수 가져오기
        GetComponent<BossDamaged>().Damaged(damage, type);
    }

    // 죽음 상태 함수
    void Die()
    {
        // 소리한번 내고
        if (dieSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(dieSound);
            Debug.Log("Die Sound");
        }

        // 임시 오브젝트 비활성화
        gameObject.SetActive(false);
        print("죽었다");
    }
}
