using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FirstLittleSister : MonoBehaviour
{

    public enum SisterState
    {
        Idle,
        Move,
        Stop
    }

    public SisterState state;

    // 빅대디의 Transform 값 가져오기
    Transform bigDaddy;
    // 추적 속도
    public float followSpeed = 5f;
    // 회전 속도
    public float rotateSpeed = 10f;
    // 유지 거리
    public float followDistance = 3f;
    // 무작위 반경
    public float wanderRadius = 7f;
    // 무작위 타이머
    public float wanderTimer = 3f;
    // 무작위 이동을 위한 시간 간격
    public float timer;
    // 무작위 이동 범위
    public float randSection = 2f;
    // 무작위 회전 반경
    public float randRotate = 1f;

    Vector3 randomPos;
    private Quaternion targetRotation;

    // 빅대디가 죽었는지?
    bool isDead = false;

    // 현재시간
    float currTime;

    // Animator
    Animator anim;
    // Nav Mesh Agent
    NavMeshAgent agent;
    // 빅대디 행동 스크립트
    private BossBehavior bossBehavior;

    // Start is called before the first frame update
    void Start()
    {
        // 빅대디의 transform 값 가져오기
        bigDaddy = GameObject.Find("BigDaddy").transform;

        // NavMeshAgent
        agent = GetComponent<NavMeshAgent>();

        // 애니메이션 컨트롤러
        anim = GetComponentInChildren<Animator>();

        // 빅대디 행동 스크립트 가져오기
        bossBehavior = GetComponent<BossBehavior>();

        // 애니메이터가 존재한다면 idle 트리거 발생
        if (anim != null)
        {
            anim.SetTrigger("Idle");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 상태
        switch (state)
        {
            case SisterState.Idle:
                Idle();
                break;
            case SisterState.Move:
                Move();
                break;
            case SisterState.Stop:
                Stop();
                break;
        }

    }

    void ChangeState(SisterState newState)
    {
        if (state != newState)
        {
            state = newState;
            switch (state)
            {
                case SisterState.Idle:
                    Idle();
                    break;
                case SisterState.Move:
                    // 애니메이션 트리거
                    if (anim != null)
                    {
                        anim.SetTrigger("Move");
                    }
                    break;
                case SisterState.Stop:
                    anim.SetTrigger("Stop");
                    break;
            }
        }
    }

    // 빅대디가 살아 있을경우, 이동 반경에 따른 대기 상태
    void Idle()
    {
        // 빅대디가 살아있다면
        if (bigDaddy != null)
        {
            // 빅대디와의 일정 거리 유지하기
            float dist = Vector3.Distance(transform.position, bigDaddy.position);
            // 빅대디가 플레이어를 추적하느라 멀어졌을 때,
            if (dist > followDistance)
            {
                // 쫓아가라
                ChangeState(SisterState.Move);
            }
            // 빅대디가 리틀시스터와 유지 거리 안에 있을 때,
            else
            {
                // 시간이 흐름
                timer += Time.deltaTime;
                // 무작위 위치로 이동
                if (timer <= wanderTimer)
                {
                    Vector3 dir = randomPos - transform.position;

                    if (dir.magnitude >= 1)
                    {
                        dir.Normalize();
                    }


                    // 이동하면서 회전하기
                    Quaternion targetRotation = Quaternion.LookRotation(dir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);

                    timer = 0;
                }
                else
                {
                    // 가만히 있지말기
                    Vector3 randomDirection = new Vector3(Random.Range(-randSection, randSection), 0, Random.Range(-randSection, randSection));
                    randomDirection += bigDaddy.position;

                    // 랜덤한 회전각 선언
                    float randAngle = Random.Range(-randRotate, randRotate);
                    Quaternion randomRotation = Quaternion.Euler(0, randAngle, 0);

                    randomPos = randomDirection;
                    timer = 0;

                    print("랜덤위치설정");

                    // 랜덤 위치 설정 후 즉시 회전 
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime);
                }
            }
        }
    }
    void Move()
    {
        // 위치 추적
        Vector3 targetPosition = bigDaddy.position;
        Vector3 smoothPosition = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        transform.position = smoothPosition;

        // 회전 추적
        Vector3 direction = bigDaddy.position - transform.position;
        direction.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);

        // 만약 빅대디가 죽으면
        if (isDead)
        {
            // 멈춤 상태로 전환한다.
            ChangeState(SisterState.Stop);
        }
    }
    void Stop()
    {
        // 이동 멈추고 SadAnimation 
        // // 가만히 그 자리에 정지한다.
        agent.isStopped = true;
        anim.SetTrigger("Stop");
        
        // 플레이어가 가까이 가서 커서를 가져갈시 구원 버튼 나오게

    }
}
