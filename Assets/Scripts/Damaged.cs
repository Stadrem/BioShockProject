﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Enemy을 위한 종합 데미지 처리 시스템입니다. Enemy의 Animator가 부착된 GameObject에 붙여주세요.
public class Damaged : MonoBehaviour
{
    //resis는 마법 저항력입니다. 0.5f로 설정하면 마법으로 인한 경직이 반감됩니다. 0으로 설정하면 경직되지 않습니다.
    public int HP = 10;
    public float resis = 1.0f;
    public float speed = 5.0f;

    //Enemy 상태 관리 가져오기
    EnemyState enemyState;

    //Enemy 가져오기
    GameObject enemy;

    //Animator 가져오기
    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        enemyState = GetComponent<EnemyState>();
        enemy = gameObject;
        anim = enemy.GetComponentInChildren<Animator>();
    }

    //받은 마법 종류에 따라서 다른 데미지와 행동을 구현합니다.
    public void Damage(int damage, string type)
    {
        switch (type)
        {
            case "Shock":
                StunDamageStep(damage, 1.0f);
                break;

            case "Fire":
                DamageStep(damage, 5, type);
                break;

            case "Ice":
                FreezeDamageStep(damage, 3.0f);
                break;

            default:
                DamageStep(damage, 1, type);
                break;
        }
    }

    //아래부터 enemyState에 상태 변화값을 던져줍니다. enemyState에 ChangeState 함수를 만들어서 컨트롤하세요.

    //기절 시간이 포함된 데미지 스텝
    void FreezeDamageStep(int damage, float time)
    {
        enemyState.ChangeState("Freeze");

        StartCoroutine(StunTime(time));
    }

    //기절 시간이 포함된 데미지 스텝
    void StunDamageStep(int damage, float time)
    {
        HP -= damage;

        enemyState.ChangeState("Stun");

        StartCoroutine(StunTime(time));
    }

    //으앙 쥬금
    void Die()
    {
        enemyState.ChangeState("Die");

        anim.SetBool("Die", true);
    }

    //damage는 깍을 피, j는 피해 입힐 횟수
    IEnumerator DamageStep(int damage, int j, string type)
    {
        enemyState.ChangeState("Damaged");

        for (int i = 0; i < j; i++)
        {
            if(type == "Melee")
            {
                HP -= damage * 2;
            }
            

            if (HP <= 0)
            {
                Die();
                yield break;
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    //스턴 시간 코루틴
    IEnumerator StunTime(float time)
    {
        anim.SetBool("IsStun", true);

        yield return new WaitForSeconds(time);

        anim.SetBool("IsStun", false);
    }

    //프리즈 시간 코루틴
    IEnumerator FreezeTime(float time)
    {
        anim.speed = 0;

        yield return new WaitForSeconds(time);

        anim.speed = 1;
    }
}