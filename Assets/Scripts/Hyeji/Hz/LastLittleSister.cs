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
    DieScript dieScript;

    AudioSource audioSource;
    public AudioClip cryingSound;

    // Start is called before the first frame update
    void Start()
    {
        // 빅대디의 transform 값 가져오기
        bigDaddy2 = GameObject.Find("BigDaddy_2").transform;
        // NavMeshAgent
        agent = GetComponent<NavMeshAgent>();
        // 애니메이션 컨트롤러
        anim = GetComponentInChildren<Animator>();

        // Audio
        audioSource = GetComponent<AudioSource>();
        //
        dieScript = bigDaddy2.GetComponent<DieScript>();

        // 애니메이터가 존재한다면 idle 트리거 발생
        if (anim != null)
        {
            anim.SetTrigger("Idle");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(dieScript.die == true)
        {
            ChangeState(SisterState.Stop);
        }


        if(isDead)
        {
            // 빅대디 죽었을 때 모든 행동 중지
            ChangeState(SisterState.Stop);
            return;
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
        if (bigDaddy2 != null && !isDead)
        {
            agent.SetDestination(bigDaddy2.position);
        }
        else
        {
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
                        audioSource.PlayOneShot(cryingSound);
                        print("출력확인");
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
        if(dieScript.die == true)
        {
            ChangeState(SisterState.Stop);
        }

    }

    void Stop()
    {
        // 빅대디의 추적을 중단하고 
        // 네비게이션 멈추고
        // 애니메이션 stop

        // 빅대디 죽었다
        if(isDead)
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
