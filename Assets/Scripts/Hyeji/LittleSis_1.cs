using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

public class LittleSis_1 : MonoBehaviour
{
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

    Vector3 randomPos;

    // 빅대디가 죽었는지?
    bool isDead = false;

    // 캐릭터 컨트롤러
    CharacterController cc;
    // 중력 적용
    private Vector3 velocity;
    private float gravity = -9.81f;

    // 현재시간
    float currTime;

    // Start is called before the first frame update
    void Start()
    {
        // 빅대디의 transform 값 가져오기
        bigDaddy = GameObject.Find("BigDaddy").transform;

        // 캐릭터 컨트롤러 컴포넌트
        cc = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        // 빅대디가 죽었는지 안죽었는지
        if (bigDaddy != null && !isDead)
        {
            Follow();
        }
        else
        {
            UnFollow();
        }

        // 중력 적용하기
        if (!cc.isGrounded)
        {
            velocity.y += gravity * Time.deltaTime;
        }
        else
        {
            velocity.y = 0;
        }
        //cc.Move(velocity * Time.deltaTime);
    }

    void Follow()
    {
        // 빅대디가 살아있다면
        if (bigDaddy != null)
        {
            // 빅대디와의 일정 거리 유지하기
            float dist = Vector3.Distance(transform.position, bigDaddy.position);
            // 빅대디와의 거리가 유지 거리보다 커질 때, 유지 거리를 벗어났을 때,
            if (dist > followDistance)
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

            }
            // 유지 거리 안에 있을 때,
            else
            {
                // 시간이 흐름
                timer += Time.deltaTime;
                // 무작위 위치로 이동
                if(timer <= wanderTimer)
                {

                    Vector3 dir = randomPos - transform.position;

                    if(dir.magnitude >= 1)
                    {
                        dir.Normalize();
                    }

                    cc.Move(dir * followSpeed * Time.deltaTime);
                }
                else
                {
                    // 가만히 있지말기
                    Vector3 randomDirection = new Vector3(Random.Range(-randSection, randSection), 0, Random.Range(-randSection, randSection));
                    randomDirection += bigDaddy.position;
                    randomPos = randomDirection;
                    timer = 0;

                    print("랜덤위치설정");
                }
            }      
        }
    }

    // 앞을 향하고 있는가?
    public bool isForward = false;

    void UnFollow()
    {
        // 빅대디가 사망처리 될 경우
        if (isDead)
        {
            // 빅대디의 뒤에 위치해서 정지 상태
            Vector3 stopDir = bigDaddy.position - transform.position;
            isForward = stopDir.z > 0 ? true : false;

        }
    }

    // 빅대디의 죽음 상태
    public void SetBigDaddyDead()
    {
        isDead = true;
    }
}
