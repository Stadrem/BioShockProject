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
    public GameObject bulletFactory;

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

        // LineRenderer 
        lr.positionCount = 2;
        lr.enabled = false;
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
                //anim.SetTrigger("WALK");
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

    private void ChangeState(EnemyState newState)
    {
        if (state == newState) return;  // 동일한 상태로의 전환을 막음

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
                break;
            case EnemyState.Damaged:
                anim.SetTrigger("DAMAGE");
                break;
            case EnemyState.Die:
                anim.SetTrigger("DIE");
                break;
        }
    }

    // 인식 거리 (플레이어가 이 거리 내로 들어오면 보스가 추적을 시작함)
    public float detectionRange = 15f;
    void Idle()
    {
        // 플레이어와의 거리 계산
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);

        // 플레이어가 인식 범위 내로 들어왔을 때 추적 시작
        if (distanceToPlayer <= detectionRange)
        {
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
            agent.SetDestination(player.position); // 플레이어를 쫓아감
            anim.SetTrigger("WALK");
        }



        ///// 응ㅇㅇㅇㅇ

        //// 플레이어와 보스 사이의 거리 구하기
        //float dist = Vector3.Distance(player.transform.position, transform.position);

        //// 플레이어가 인지 범위 내로 들어왔다면
        //if (dist < findDistance)
        //{
        //    // 플레이어를 부드럽게 바라보도록 회전
        //    Vector3 directionToPlayer = player.position - transform.position;
        //    // 수직 회전 방지
        //    directionToPlayer.y = 0;

        //    Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
        //    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

        //    agent.SetDestination(player.position);


        //    if(dist < attackDistance)
        //    {
        //        print("왜안움직니느데?");
        //        ChangeState(EnemyState.Attack);           
        //    }      
        //}

        //agent.SetDestination(player.position);
    }

    // 공격 상태 함수
    void Attack()
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

    void AttackRay(Vector3 aimPos)
    {

        // 플레이어가 있는 방향으로 ray를 발사한다.
        Ray ray = new Ray(firePos.transform.position, aimPos - firePos.transform.position);
        RaycastHit hit;

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
        // 보스 데미지드 함수 가져오기
        GetComponent<BossDamaged>().Damaged(damage, type);
    }

    // 죽음 상태 함수
    void Die()
    {
        // 임시 오브젝트 비활성화
        gameObject.SetActive(false);
        print("죽었다");
    }
}
