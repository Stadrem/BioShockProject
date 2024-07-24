using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //여러곳에서 불러올 수 있게 public, 그리고 static으로 선언. (GameManager를 담는 변수)
    public static GameManager instance;

    //다른 곳에서 활용할 변수등등
    public int HP;

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
    }

    //여러번, 고정적으로 사용할 함수 생성
    public void UpdateScore(int addScore)
    {
        //처리할 내용들
        
    }
}