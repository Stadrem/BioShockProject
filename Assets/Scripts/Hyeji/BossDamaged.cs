using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.UI;

public class BossDamaged : MonoBehaviour
{
    // 최대 HP
    public int maxHP = 100;
    // 현재 HP
    public int currHP;

    // HP UI
    public Slider hpUI;

    // 보스행동 스크립트
    private BossBehavior bossBehavior;
    // 보스행동 스크립트
    private BossBehavior_2 bossBehavior2;

    // Start is called before the first frame update
    void Start()
    {
        // bossBehavior 스크립트 참조
        bossBehavior = GetComponent<BossBehavior>();
        // bossBehavior_2 스크립트 참조
        bossBehavior2 = GetComponent<BossBehavior_2>();

        // 현재 HP를 최대 HP로 설정하자
        currHP = maxHP;

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Damaged(int damage, string type)
    {
        // 체력이 0 이하인지 확인
        if(currHP <= 0)
        {
            currHP = 0;
            bossBehavior.ChangeState(BossBehavior.EnemyState.Die);
        }

        // HP 바를 갱신하자.
        float ratio = currHP * 0.01f;
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
        if (currHP <= 0)
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
        currHP -= damage; //동결 상태에서 데미지 처리 (값은 필요에 따라 조정)
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
        currHP -= damage;
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
                    currHP -= damage * 2;
                }
                else
                {
                    // 아닐 시, 데미지 감소
                    currHP -= damage;
                }
                if (currHP <= 0)
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
