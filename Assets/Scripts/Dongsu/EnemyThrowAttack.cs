using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyAttack;

public class EnemyThrowAttack : MonoBehaviour, IAttack
{
    //타겟 위치 임시 저장
    Vector3 tempPosition;

    EnemyState enemyState;

    public GameObject bombItem;

    public GameObject attackPoint;

    public GameObject attackEffect;

    private void Start()
    {
        enemyState = GetComponentInParent<EnemyState>();
    }

    public void Attack()
    {
        attackEffect.SetActive(true);

        print("공격 호출!");
        tempPosition = GameManager.instance.player.transform.position;

        float distance = Vector3.Distance(tempPosition, transform.position);

        //상대와 나의 거리가 reAttackDistance보다 크면, 추적
        if (distance > enemyState.attackRanage + 5)
        {
            enemyState.ChangeState(EnemyState.State.Chase);
        }

        StartCoroutine(AttackDelay());
    }

    IEnumerator AttackDelay()
    {
        print("공격 레이 쏨 ");
        yield return new WaitForSeconds(0.05f);

        RaycastHit hit;

        Vector3 dir = tempPosition - attackPoint.transform.position;
        dir.Normalize();

        if (Physics.Raycast(attackPoint.transform.position, dir, out hit, enemyState.attackRanage, enemyState.layerMask))
        {
            //Debug.DrawRay(attackPoint.transform.position, hit.transform.position - attackPoint.transform.position, Color.green, 1.0f);
            if (hit.transform.CompareTag("Player"))
            {
                GameObject bombInstance = Instantiate(bombItem);

                bombInstance.transform.position = attackPoint.transform.position;

                Rigidbody rb = bombInstance.GetComponent<Rigidbody>();

                bombInstance.transform.forward = dir;

                rb.velocity = bombInstance.transform.forward * 15;

                print("던짐!");
            }
            else
            {
                enemyState.ChangeState(EnemyState.State.Chase);
            }
        }
        else
        {
            print("공격 레이 안 맞음");
            enemyState.ChangeState(EnemyState.State.Chase);
        }
        yield return new WaitForSeconds(0.9f);
        attackEffect.SetActive(false);
    }
}
