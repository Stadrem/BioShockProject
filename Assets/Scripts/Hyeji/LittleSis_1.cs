using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

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
    // 무작위 이동 대기 시간 
    public float wanderTimer = 2f;

    public float timer;
    // 빅대디가 죽었는지?
    bool isDead = false;

    // Start is called before the first frame update
    void Start()
    {
        // 빅대디의 transform 값 가져오기
        bigDaddy = GameObject.Find("BigDaddy").transform;
        timer = wanderTimer;
    }

    // Update is called once per frame
    void Update()
    {
        // 빅대디가 죽었는지 안죽었는지
        if(isDead)
        {
            Follow();
        }
        else
        {
            UnFollow();
        }
    }

    void Follow()
    {
        // 빅대디의 뒤를 쫓아다닌다.
        if (bigDaddy != null)
        {
            // 가만히 있는게 아닌 계속 일정한 구역을 돌아다닌다.
            float randSection = Random.Range(0.5f, 1.5f);

            // 빅대디와의 일정 거리 유지하기
            float dist = Vector3.Distance(transform.position, bigDaddy.position);
            // 빅대디와의 거리가 유지 거리보다 커질 때, 유지 거리를 벗어났을 때,
            if (dist > followDistance)
            {
                // 위치 추적
                Vector3 targetPosition = bigDaddy.position;
                Vector3 smoothPosition = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
                transform.position = smoothPosition;
            }
            // 유지 거리 안에 있을 때,
            else
            {
                // 무작위 이동
                timer += Time.deltaTime;
            }

            // 회전 추적
            Vector3 direction = bigDaddy.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
        }
    }

    void UnFollow()
    {
        // 빅대디가 사망처리 될 경우
        if(isDead)
        {
            // 빅대디의 뒤에 위치해서 정지 상태
        }

        // 구원이 끝나면 다른 장소(특정 오브젝트 설정)로 도망감

    }

    // 빅대디의 죽음 상태
    public void SetBigDaddyDead()
    {
        
    }
}
