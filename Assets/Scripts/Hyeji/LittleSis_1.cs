using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LittleSis_1 : MonoBehaviour
{
    // 빅대디의 Transform 값 가져오기
    Transform bigDaddy;
    // 추적 속도
    public float followSpeed = 5f;
    // 회전 속도
    public float rotateSpeed = 10f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // 빅대디의 transform 값 가져오기
        bigDaddy = GameObject.Find("BigDaddy").transform;

        // 빅대디의 뒤를 쫓아다닌다.
        if (bigDaddy != null)
        {
            // 빅대디와의 일정 거리 유지하기


            // 위치 추적
            Vector3 targetPosition = bigDaddy.position;
            Vector3 smoothPosition = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
            transform.position = smoothPosition;

            // 회전 추적
            Vector3 direction = bigDaddy.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
        }

    }

    void UnFollow()
    {
        // 빅대디가 사망처리 될 경우
        // 플레이어와 일정 거리를 유지하며 도망다닌다.
        // 플레이어와의 거리가 가까워지면
        // 플레이어에게 잡힘

    }
}
