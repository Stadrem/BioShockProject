using System.Collections;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.AI;

public class BossBehavior_2 : MonoBehaviour
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
    EnemyState state;

    // Player 의 Transform
    Transform player;

    // 로지가 이동할 수 있는 반경
    public float patrolRadius = 10f;
    // 새로운 목표 지점을 설정할 시간 간격
    public float patrolTime = 5f;

    // NavMeshAgent
    private NavMeshAgent agent;
    // 로지의 시작 지점
    private Vector3 startPosition;

    public float findDistance = 30f;
    public float stopDistance = 15f;
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
        // 플레이어의 Transform 찾자
        player = GameObject.Find("Player").transform;
        // NavMeshAgent 컴포넌트
        agent = GetComponent<NavMeshAgent>();
        agent.isStopped = false;
        // 로지의 시작 지점 저장
        startPosition = transform.position;

        // 초기 상태를 Idle로 설정
        ChangeState(EnemyState.Idle);
        //StartCoroutine(Patrol());
    }

    void Update()
    {
        switch (state)
        {
            case EnemyState.Idle:
                break;
            case EnemyState.Move:
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

    private void ChangeState(EnemyState newState)
    {
        state = newState;
        StopAllCoroutines();

        switch (newState)
        {
            case EnemyState.Idle:
                StartCoroutine(Idle());
                break;
            case EnemyState.Move:
                agent.isStopped = false;
                StartCoroutine(Patrol());
                break;
            case EnemyState.Attack:
                // 공격 시에는 navmesh 멈추자
                agent.isStopped = false;
                break;
            case EnemyState.Damaged:
                break;
            case EnemyState.Die:
                break;
        }
    }

    // 주어진 반경 내에서 랜덤한 위치를 반환하는 함수
    private Vector3 GetRandomPoint(Vector3 center, float radius)
    {
        Vector3 randomPos = Random.insideUnitSphere * radius;
        randomPos += center;

        NavMeshHit hit;
        NavMesh.SamplePosition(randomPos, out hit, radius, 1);

        return hit.position;
    }

    // 대기 상태 함수
    private IEnumerator Idle()
    {
        yield return new WaitForSeconds(2f);
        // Move 상태로 전환한다. 아 이걸 Patrol로?
        ChangeState(EnemyState.Move);
    }

    // 정찰 상태 함수
    private IEnumerator Patrol()
    {
        while (state == EnemyState.Move)
        {
            Vector3 newDestination = GetRandomPoint(startPosition, patrolRadius);
            agent.SetDestination(newDestination);

            yield return new WaitForSeconds(patrolTime);
        }
    }

    // 이동 상태 함수
    void Move()
    {

        // 플레이어와 보스 사이의 거리 구하기
        float dist = Vector3.Distance(player.transform.position, transform.position);

        // 플레이어가 존재한다면
        if (player != null)
        {
            // 플레이어를 부드럽게 바라보도록 회전
            Vector3 directionToPlayer = player.position - transform.position;
            // 수직 회전 방지
            directionToPlayer.y = 0; 

            Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

            // 플레이어가 인지 범위 내로 들어왔다면
            if(dist < findDistance)
            {
                ChangeState(EnemyState.Attack);
            }

            // 플레이어와의 거리가 공격 가능 범위 내로 들어온다면
            if (dist < attackDistance)
            {
                // 공격 상태로 전환
                ChangeState(EnemyState.Attack);
            }
        }
    }

    // 공격 상태 함수
    void Attack()
    {
        // 플레이어를 바라보기
        Vector3 directionToPlayer = player.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 3f);

        // 보스와 플레이어 거리
        float dist = Vector3.Distance(player.transform.position, transform.position);

        // 시간을 흐르게 한다.
        currTime += Time.deltaTime;
        if (currTime >= attackDelayTime)
        {
            // Raycast를 이용한 공격 패턴(원거리)
            AttackRay(player.transform.position);

            // 싱글톤으로 HP 관리
            GameManager.instance.Damaged(attackPower);

            // 현재 시간을 초기화 해준다
            currTime = 0;

            // 플레이어와의 거리가 공격범위에서 벗어난다면 재추적
            if (dist >= attackDistance)
            {
                ChangeState(EnemyState.Move);
            }
            // 플레이어와 거리가 공격 내에 있지만 멈추는 거리 밖에 있을 경우 추적
            else if (dist > stopDistance)
            {
                // 플레이어를 추적하도록 이동
                if (!agent.isStopped)
                {
                    agent.SetDestination(player.position);
                }
            }
            // 플레이어가 멈추는 거리 내에 있으면 멈춰라
            else if (dist <= stopDistance)
            {
                // 움직임을 활성화한다.
                agent.isStopped = true;
            }
        }

        else
        {
            // 공격 중에도 플레이어를 추적하자
            if (!agent.isStopped)
            {
                agent.SetDestination(player.position);
            }
        }
    }

    void AttackRay(Vector3 aimPos)
    {
        // 플레이어가 있는 방향으로 ray를 발사한다.
        Ray ray = new Ray(firePos.transform.position, aimPos - firePos.transform.position);
        RaycastHit hitInfo;

        // Raycast 거리 설정
        float rayDistance = 20f;
        if (Physics.Raycast(ray, out hitInfo, rayDistance))
        {
            // 총알을 생성하자.
            GameObject bullet = Instantiate(bulletFactory);
            // 맞은 위치에 두기
            bullet.transform.position = firePos.transform.position;
            // 총알의 방향을 레이의 방향으로
            bullet.transform.forward = (hitInfo.point - firePos.transform.position).normalized;

            print("공격해");

            // 파편효과 2초뒤에 파괴
            Destroy(bullet, 2f);

            // 맞은 대상이 Player라면
            if (hitInfo.collider.CompareTag("Player"))
            {
                // Player 에게 데미지를 주자
                GameManager.instance.Damaged(attackPower);
            }
        }
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
