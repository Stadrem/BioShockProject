using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.UI;

public class BossDamaged : MonoBehaviour
{
    // HP 상태 100
    public int HP = 100;
    // 현재 HP
    public int currHP = 0;

    // HP UI
    public Slider hpUI;

    // 보스행동 스크립트
    private BossBehavior bossBehavior;

    // Start is called before the first frame update
    void Start()
    {
        // bossBehavior 스크립트 참조
        bossBehavior = GetComponent<BossBehavior>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Damaged(int damage, string type)
    {
        // HP 바를 갱신하자.
        int ratio = currHP / HP;
        hpUI.value = ratio;

        switch (type)
        {
            case "Shock":
                StartCoroutine(StunDamageStep(damage, 1.0f));
                break;
            case "Fire":
                StartCoroutine(DamageStep(damage, 5, type));
                break;
            case "Ice":
                StartCoroutine(FreezeDamageStep(3, 3.0f));
                break;
            default:
                StartCoroutine(DamageStep(damage, 1, type));
                break;
        }
        CheckIfDead();
    }
    // 사망 유무 판단 함수
    private void CheckIfDead()
    {
        // 적이 죽었는지 확인한다.
        if (HP <= 0)
        {
            bossBehavior.ChangeState(BossBehavior.EnemyState.Die);
        }
        else
        {
            bossBehavior.ChangeState(BossBehavior.EnemyState.Damaged);
        }
    }
    //  동결 상태
    IEnumerator FreezeDamageStep(int damage, float freezeDuration)
    {
        HP -= damage; //동결 상태에서 데미지 처리 (값은 필요에 따라 조정)
        print("동결 상태");

        // 동결 상태 적용
        bossBehavior.ChangeState(BossBehavior.EnemyState.Damaged);
        // 이동 멈춤
        // 애니메이션 멈춤

        yield return new WaitForSeconds(freezeDuration);

        // 동결 상태 해제
        // 이동 재시작
        // 애니메이션 시작

        // 대기 상태로 전환
        bossBehavior.ChangeState(BossBehavior.EnemyState.Idle); 
    }

    // 감전 상태
    IEnumerator StunDamageStep(int damage, float stunDuration)
    {
        HP -= damage;
        print("감전");

        // 스턴 상태 적용
        bossBehavior.ChangeState(BossBehavior.EnemyState.Damaged);
        // 애니메이션 삽입

        yield return new WaitForSeconds(stunDuration);

        // 애니메이션 삽입
        // 대기 상태 변환
        bossBehavior.ChangeState(BossBehavior.EnemyState.Idle);
    }


    // 근접, 원거리 공격 상태
    // 근접 피해량 2배 증가
    IEnumerator DamageStep(int damage, int j, string type)
    {
        bossBehavior.ChangeState(BossBehavior.EnemyState.Damaged);
        {
            for(int i = 0; i < j; i++)
            {
                // 근접 공격일 경우 피해량 2배 증가
                if(type == "Melee")
                {
                    HP -= damage * 2;
                }
                else
                {
                    // 아닐 시, 데미지 감소
                    HP -= damage;
                }
                if (HP <= 0)
                {
                    bossBehavior.ChangeState(BossBehavior.EnemyState.Die);
                    yield break;
                }
                yield return new WaitForSeconds(0.5f);
            }

            // 대기 상태로 전환한다.
            bossBehavior.ChangeState(BossBehavior.EnemyState.Idle);
        }
    }
}
