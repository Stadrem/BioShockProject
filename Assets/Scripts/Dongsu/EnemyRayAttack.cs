using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyAttack;

public class EnemyRayAttack : MonoBehaviour, IAttack
{
    //타겟 위치 임시 저장
    Vector3 tempPosition;

    EnemyState enemyState;

    public GameObject attackPoint;

    public GameObject attackEffect;

    AudioSource audioSource;

    private void Start()
    {
        enemyState = GetComponentInParent<EnemyState>();
        audioSource = GetComponent<AudioSource>();
    }

    public void Attack()
    {
        audioSource.Play();

        attackEffect.SetActive(true);

        enemyState.WaitStop();

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
        yield return new WaitForSeconds(0.15f);

        RaycastHit hit;

        if (Physics.Raycast(attackPoint.transform.position, tempPosition - attackPoint.transform.position, out hit, enemyState.attackRanage, enemyState.layerMask))
        {
            Debug.DrawRay(attackPoint.transform.position, hit.transform.position - attackPoint.transform.position, Color.green, 1.0f);
            if (hit.transform.CompareTag("Player"))
            {
                GameManager.instance.Damaged(1);
            }
            else
            {
                AttackFailed();
            }
        }
        else
        {
            AttackFailed();
        }
        yield return new WaitForSeconds(0.6f);
        attackEffect.SetActive(false);
    }
    void AttackFailed()
    {
        enemyState.ChangeState(EnemyState.State.Chase);
    }
}