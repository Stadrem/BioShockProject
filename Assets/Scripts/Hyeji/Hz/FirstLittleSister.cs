using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    // 빅대디의 Transform
    Transform bigDaddy;

    // 유지 거리
    public float followDistance = 3f;

    // 빅대디가 죽었는지?
    bool isDead = false;

    // 현재시간
    float currTime;

    // Animator
    Animator anim;
    // Nav Mesh Agent
    NavMeshAgent agent;

    DieScript dieScript;

    void Start()
    {
        // 빅대디의 transform 값 가져오기
        bigDaddy = GameObject.Find("BigDaddy").transform;
        // NavMeshAgent
        agent = GetComponent<NavMeshAgent>();
        // 애니메이션 컨트롤러
        anim = GetComponentInChildren<Animator>();

        //
        dieScript = bigDaddy.GetComponent<DieScript>();

        // 애니메이터가 존재한다면 idle 트리거 발생
        if (anim != null)
        {
            anim.SetTrigger("Idle");
        }
    }

    void Update()
    {
        if (dieScript.die == true)
        {
            ChangeState(SisterState.Stop);
        }

        // 빅대디 죽으면 Stop 함수로 호출
        if (isDead)
        {
            ChangeState(SisterState.Stop);
            return;
        }


        // 빅대디가 있는 방향으로 몸을 회전시킨다.
        Vector3 directionToBigDaddy = bigDaddy.transform.position - transform.position;
        directionToBigDaddy.y = 0;

        if (directionToBigDaddy != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(directionToBigDaddy);
            // 보간을 이용하여 속도 조절
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 1f);
        }

        // 빅대디 죽으면 추적 다 중단
        // 빅대디가 살아있고 추적 상태일때
        if (bigDaddy != null && !isDead)
        //if (bigDaddy != null & !isDead && state == SisterState.Move)
        {
            // 빅대디의 위치로 간다
            agent.SetDestination(bigDaddy.transform.position);
        }
        // 빅대디가 죽었고 추적이 중단된다면
        else
        {
            // 이동을 멈춘다 (이거 바꿨음 0812)
            //agent.isStopped = false;
            agent.isStopped = true;
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
        // 빅대디가 살아있고 
        if (bigDaddy != null && !isDead)
        {
            // 빅대디와의 거리 계산
            float dist = Vector3.Distance(transform.position, bigDaddy.transform.position);

            // 빅대디와의 거리가 인지거리보다 크면 Move 상태로 전환
            if (dist > followDistance)
            {
                agent.SetDestination(bigDaddy.transform.position);
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
        // 빅대디 있고, 살아있으면
        if (bigDaddy != null && !isDead)
        {
            // 빅대디와의 현재 거리 계산
            float dist = Vector3.Distance(transform.position, bigDaddy.transform.position);

            // 빅대디와의 거리 계산
            if (dist <= followDistance)
            {
                ChangeState(SisterState.Idle);
            }

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
        if (dieScript.die == true)
        {
            ChangeState(SisterState.Stop);
        }
    }

    void Stop()
    {
        // 빅대디 죽었다
        if (isDead)
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

    }
}
