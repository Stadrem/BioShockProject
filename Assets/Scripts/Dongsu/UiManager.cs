using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public static UiManager instance;

    public Image HPGauge;
    int maxHP;
    float currentHP;

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

    // Start is called before the first frame update
    void Start()
    {
        maxHP = GameManager.instance.HP;
        currentHP = maxHP;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HPRefresh(int i)
    {
        currentHP = i;
        HPGauge.fillAmount = currentHP * 0.1f;
    }
}
