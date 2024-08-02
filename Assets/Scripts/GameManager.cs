using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //여러곳에서 불러올 수 있게 public, 그리고 static으로 선언. (GameManager를 담는 변수)
    public static GameManager instance;

    //다른 곳에서 활용할 변수등등
    public int HP = 10;
    public int maxHP;

    public GameObject player;

    Animator anim;

    public float shakeAmount = 0.07f;

    bool shake;

    private Transform cameraTransform;
    private Vector3 originalCameraLocalPosition;

    private void Awake()
    {
        //instance 값이 null이면
        if (instance == null)
        {
            //이 스크립트를 instance에 담음
            instance = this;

            //씬 전환해도 유지하는 코드
            DontDestroyOnLoad(gameObject);
        }
        //이미 instance에 무언가 값이 들어있다면?
        else
        {
            //의도치 않은 중복 적용일 태니 이 게임 오브젝트 파괴.
            Destroy(gameObject);
        }

        if (GameObject.Find("Player") == null)
        {
            Debug.LogError("Player GameObject not found!");
            // 추가적인 예외 처리 코드
        }
        else
        {
            player = GameObject.Find("Player");
        }

        cameraTransform = Camera.main.transform.parent;
        originalCameraLocalPosition = Camera.main.transform.localPosition;
        anim = player.GetComponentInChildren<Animator>();
        maxHP = HP;
    }

    private void Update()
    {
        if (shake == true)
        {
            Camera.main.transform.localPosition = originalCameraLocalPosition + Random.insideUnitSphere * shakeAmount;
        }
    }

    //여러번, 고정적으로 사용할 함수 생성
    public void Damaged(int num)
    {
        HP -= num;

        if(HP > maxHP)
        {
            HP = maxHP;
        }
        UiManager.instance.HPRefresh(HP);

        StartCoroutine(ShakeTime());
    }

    IEnumerator ShakeTime()
    {
        shake = true;

        yield return new WaitForSeconds(0.1f);

        shake = false;
        Camera.main.transform.localPosition = originalCameraLocalPosition;
    }
}