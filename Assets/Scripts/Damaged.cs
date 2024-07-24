using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Enemy을 위한 종합 데미지 처리 시스템입니다.
public class Damaged : MonoBehaviour
{
    //resis는 마법 저항력입니다. 0.5f로 설정하면 마법으로 인한 경직이 반감됩니다. 0으로 설정하면 경직되지 않습니다.
    public int HP = 10;
    public float resis = 1.0f;

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

    public void Damage(int damage, string type)
    {
        switch (type)
        {
            case "Shock":
                StunDamageStep(damage);
                break;

            case "Fire":
                DamageStep(damage, 5);
                break;

            case "Ice":
                StunDamageStep(damage);
                break;

            default:
                DamageStep(damage, 1);
                break;
        }
    }

    void StunDamageStep(int damage)
    {
        HP -= damage;
        enemyState.ChangeState(EnemyState.State.Stun);
        anim.SetTrigger("IsStun");
    }

    void Die()
    {
        enemyState.ChangeState(EnemyState.State.Die);
        anim.SetTrigger("IsDie");
    }

    //damage는 깍을 피, j는 피해 입힐 횟수
    IEnumerator DamageStep(int damage, int j)
    {
        for (int i = 0; i < j; i++)
        {
            HP -= damage;

            if (HP <= 0)
            {
                Die();
                yield break;
            }

            yield return new WaitForSeconds(0.5f);
        }
    }
}
