using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class LastLittleSister : MonoBehaviour
{
    public enum SisterState
    {
        Idle,
        Move,
        Stop
    }

    public SisterState state;

    // 빅대디의 Transform 값 가져오기
    Transform bigDaddy2;
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
    // 로지 행동 스크립트
    private rosieBehavior rosie;


    // Start is called before the first frame update
    void Start()
    {
        // 빅대디의 transform 값 가져오기
        bigDaddy2 = GameObject.Find("BigDaddy_2").transform;
        // NavMeshAgent
        agent = GetComponent<NavMeshAgent>();
        // 애니메이션 컨트롤러
        anim = GetComponentInChildren<Animator>();
        // 빅대디 행동 스크립트 가져오기
        rosie = GetComponent<rosieBehavior>();

        // 애니메이터가 존재한다면 idle 트리거 발생
        if (anim != null)
        {
            anim.SetTrigger("Idle");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 빅대디 죽으면 회전시키는 로직도 중단
        // 빅대디가 살아있고, 죽지 않은 경우에만 회전 및 이동처리를 해야함 (변경 0812, 0813)
        if (bigDaddy2 != null & isDead)
        {
            return;
        }
        // 만약 빅대디 죽으면 바로 상태전환을 Stop으로
        else
        {
            // 빅대디 죽으면 Stop 함수로 호출
            ChangeState(SisterState.Stop);
        }


        // 빅대디가 있는 방향으로 몸을 회전시킨다.
        Vector3 directionToBigDaddy = bigDaddy2.position - transform.position;
        directionToBigDaddy.y = 0;

        if (directionToBigDaddy != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(directionToBigDaddy);
            // 보간을 이용하여 속도 조절
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 1f);
        }

        // 빅대디 죽으면 추적 다 중단
        if (bigDaddy2 != null & !isDead && state == SisterState.Move)
        {
            agent.SetDestination(bigDaddy2.position);
        }
        else
        {
            agent.isStopped = false;
        }

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
            // 상태 전환 로그
            Debug.Log($"State changing from {state} to {newState}");
            state = newState;
            switch (state)
            {
                case SisterState.Idle:
                    if (anim != null)
                    {
                        anim.ResetTrigger("Move");
                        anim.SetTrigger("Idle");
                    }
                    break;
                case SisterState.Move:
                    if (anim != null)
                    {
                        anim.ResetTrigger("Idle");
                        anim.SetTrigger("Move");
                    }
                    break;
                case SisterState.Stop:
                    if (anim != null)
                    {
                        anim.SetTrigger("Stop");
                    }
                    break;
            }
        }
    }

    // 빅대디가 살아 있을경우, 이동 반경에 따른 대기 상태
    void Idle()
    {
        if (bigDaddy2 != null && !isDead)
        {
            // 빅대디와의 거리 계산
            float dist = Vector3.Distance(transform.position, bigDaddy2.position);

            // 빅대디와의 거리가 인지거리보다 크면 Move 상태로 전환
            if (dist > followDistance)
            {
                agent.SetDestination(bigDaddy2.position);
                ChangeState(SisterState.Move);
            }
            else
            {
                if (anim != null)
                {
                    anim.SetTrigger("Idle");
                }
            }
        }
    }
    void Move()
    {
        // 빅대디가 살아있고 죽지 않았다면
        if (bigDaddy2 != null && !isDead)
        {
            // 빅대디와의 현재 거리 계산
            float dist = Vector3.Distance(transform.position, bigDaddy2.position);

            // 빅대디와의 거리 계산해서 가까우면 대기
            if (dist <= followDistance)
            {
                ChangeState(SisterState.Idle);
            }
            // 만약 빅대디랑 거리가 멀어지면 따라가기
            else
            {
                // 거리가 충분히 가까워지면 Idle 상태로 전환
                if (state != SisterState.Move)
                {
                    ChangeState(SisterState.Move);

                    // Idle 애니메이션 트리거 설정
                    if (anim != null)
                    {
                        anim.SetTrigger("Move");
                    }
                }
            }
        }
        // 빅대디가 사라지고 죽었다면
        else
        {
            // Stop 상태로 전환한다(울어)
            ChangeState(SisterState.Stop);
        }


        // 잠만잠만
        //// 빅대디가 살아있고 죽지 않았다면
        //if(bigDaddy != null & !isDead)
        //{
        //    //agent.SetDestination(bigDaddy.position);
        //    //ChangeState(SisterState.Move);

        //    //if (anim != null)
        //    //{
        //    //    anim.SetTrigger("Move");
        //    //}

        //    // 상태가 이미 Move일 경우 다시 상태 전환을 하지 않음
        //    if (state != SisterState.Move)
        //    {
        //        ChangeState(SisterState.Move);

        //        if (anim != null)
        //        {
        //            anim.SetTrigger("Move");
        //        }
        //    }
        //    // 빅대디 추적 따라가
        //    agent.SetDestination(bigDaddy.position);


        //    float dist = Vector3.Distance(transform.position, bigDaddy.position);
        //    // 만약 추적거리 안이라면
        //    if(dist <= followDistance)
        //    {
        //        // 대기 상태로 가라
        //        ChangeState(SisterState.Idle);
        //    }
        //    else
        //    {
        //        if(state != SisterState.Move)
        //        {
        //            ChangeState(SisterState.Move);
        //            anim.SetTrigger("Move");
        //        }
        //    }
        //}
        //// 빅대디 죽으면
        //else
        //{
        //    // 울어라
        //    ChangeState(SisterState.Stop);
        //}
        // 여기여기까지 살려

        //// 위치 추적
        //Vector3 targetPosition = bigDaddy.position;
        //Vector3 smoothPosition = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        //transform.position = smoothPosition;

        //// 회전 추적
        //Vector3 direction = bigDaddy.position - transform.position;
        //direction.y = 0;
        //Quaternion targetRotation = Quaternion.LookRotation(direction);
        //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);

        //// 만약 빅대디가 죽으면
        //if (isDead)
        //{
        //    // 멈춤 상태로 전환한다.
        //    ChangeState(SisterState.Stop);
        //}
    }

    void Stop()
    {
        // 빅대디의 추적을 중단하고 
        // 네비게이션 멈추고
        // 애니메이션 stop

        // 빅대디 죽었다
        if(!isDead)
        {
            // 근데 에이전트가 존재하면
            if (agent != null)
            {
                // 이동을 멈추기
                agent.isStopped = true;
                // 에이전트 경로를 초기화시킨다.
                agent.ResetPath();
            }
            // anim 존재하면
            if (anim != null)
            {
                // Stop 애니메이션 작동
                anim.SetTrigger("Stop");
            }
        }
        


        // 이동 멈추고 SadAnimation 
        // // 가만히 그 자리에 정지한다.
        //agent.isStopped = true;
        //anim.SetTrigger("Stop");

        // 플레이어가 가까이 가서 커서를 가져갈시 구원 버튼 나오게

    }
}
