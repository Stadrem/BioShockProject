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

    private void Start()
    {
        enemyState = GetComponentInParent<EnemyState>();
    }

    public void Attack()
    {
        print("공격 호출!");
        tempPosition = GameManager.instance.player.transform.position;

        float distance = Vector3.Distance(tempPosition, transform.position);

        //상대와 나의 거리가 reAttackDistance보다 크면, 추적
        if (distance > enemyState.attackRanage)
        {
            enemyState.ChangeState(EnemyState.State.Chase);
        }

        StartCoroutine(AttackDelay());
    }

    IEnumerator AttackDelay()
    {
        print("공격 레이 쏨 ");
        yield return new WaitForSeconds(0.01f);

        RaycastHit hit;

        if (Physics.Raycast(transform.position, tempPosition - transform.position, out hit, enemyState.attackRanage, enemyState.layerMask))
        {
            Debug.DrawRay(transform.position, hit.transform.position - transform.position, Color.green, 1.0f);
            if (hit.transform.CompareTag("Player"))
            {
                GameObject bombInstance = Instantiate(bombItem);

                bombInstance.transform.position = transform.position;

                Rigidbody rb = bombInstance.GetComponent<Rigidbody>();

                print("던짐!");
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
                enemyState.ChangeState(EnemyState.State.Chase);
            }
        }
        else
        {
            print("공격 레이 안 맞음");
            yield return new WaitForSeconds(0.5f);
            enemyState.ChangeState(EnemyState.State.Chase);
        }
    }
}
