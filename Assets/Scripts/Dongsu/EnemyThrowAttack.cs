using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
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

        tempPosition = GameManager.instance.player.transform.position;

        float distance = Vector3.Distance(tempPosition, transform.position);

        Vector3 dir = GameManager.instance.player.transform.position - transform.position;
        dir.Normalize();

        //상대와 나의 거리가 reAttackDistance보다 크면, 추적
        if (distance > enemyState.attackRanage + 5)
        {
            enemyState.ChangeState(EnemyState.State.Chase);
        }
        StartCoroutine(AttackDelay(dir));
    }

    IEnumerator AttackDelay(Vector3 dir)
    {
        yield return new WaitForSeconds(0.05f);

        enemyState.attackPass = true;

        GameObject bombInstance = Instantiate(bombItem);

        bombInstance.transform.position = attackPoint.transform.position;

        Rigidbody rb = bombInstance.GetComponent<Rigidbody>();

        bombInstance.transform.forward = dir;

        rb.velocity = bombInstance.transform.forward * 10;

        yield return new WaitForSeconds(0.9f);
        attackEffect.SetActive(false);
    }
}

