using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDamaged : MonoBehaviour
{
    public int HP = 100;

    // 에너미 상태 가져오기
    EnemyState enemyState;

    // Start is called before the first frame update
    void Start()
    {
        enemyState = GetComponent<EnemyState>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Damaged(int damage, string type)
    {
        switch (type)
        {
            case "Shock":
                StunDamageStep(damage, 1.0f);
                break;
            case "Fire":
                StartCoroutine(DamageStep(damage, 5, type));
                break;
            case "Ice":
                FreezeDamageStep(3, 3.0f);
                break;
            default:
                StartCoroutine(DamageStep(damage, 1, type));
                break;
        }

        // 적이 죽었는지 확인한다.
        if(HP <= 0)
        {
            enemyState.ChangeState(EnemyState.State.Die);
        }
        else
        {
            enemyState.ChangeState(EnemyState.State.Damaged);
        }
    }
    //  동결 상태
    IEnumerator FreezeDamageStep(int damage, float freezeDuration)
    {
        HP -= 3; // 동결 상태에서 데미지 처리 (값은 필요에 따라 조정)
        print("동결 상태");

        // 동결 상태 적용
        enemyState.ChangeState(EnemyState.State.Damaged);
        // 이동 멈춤
        // 애니메이션 멈춤

        yield return new WaitForSeconds(freezeDuration);

        // 동결 상태 해제
        // 이동 재시작
        // 애니메이션 시작

        enemyState.ChangeState(EnemyState.State.Idle); // 대기 상태로 전환
    }
    //void FreezeDamageStep(int damage)
    //{
    //    HP -= (int)damage;
    //    print("동결");
    //}

    // 감전 상태
    IEnumerator StunDamageStep(int damage, float stunDuration)
    {
        HP -= damage;
        print("감전");

        // 스턴 상태 적용
        enemyState.ChangeState(EnemyState.State.Damaged);
        // 애니메이션 삽입

        yield return new WaitForSeconds(stunDuration);

        // 애니메이션 삽입
        // 대기 상태 변환
        enemyState.ChangeState(EnemyState.State.Idle);
    }

    //void StunDamageStep(int damage, float stunDuration)
    //{
    //    HP -= damage;
    //    print("기절");

    //    // 적의 상태를 기절로 설정
    //}


    // 근접, 원거리 공격 상태
    // 근접 피해량 2배 증가
    IEnumerator DamageStep(int damage, int j, string type)
    {
        enemyState.ChangeState(EnemyState.State.Damaged);
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
                    enemyState.ChangeState(EnemyState.State.Die);
                    yield break;
                }
                yield return new WaitForSeconds(0.5f);
            }

            // 대기 상태로 전환한다.
            enemyState.ChangeState(EnemyState.State.Idle);
        }
    }
}
