using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEditor.PackageManager;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class AnimEvent : MonoBehaviour
{
    EnemyState enemyState;

    public float attackRanage = 7.0f;

    public GameObject attackPoint;

    //Animator 가져오기
    Animator anim;

    //타겟 위치 임시 저장
    Vector3 tempPosition;

    private void Start()
    {
        enemyState = GetComponentInParent<EnemyState>();
        anim = GetComponent<Animator>();
    }

    void IsAttack()
    {
        //Vector3 tempPosition = GameManager.instance.player.transform.position;

        float distance = Vector3.Distance(tempPosition, transform.position);

        //상대와 나의 거리가 reAttackDistance보다 크면, 추적
        if (distance > enemyState.reAttackDistance)
        {
            enemyState.ChangeState(EnemyState.State.Chase);
        }

        StartCoroutine(AttackDelay());
    }

    void IsDamaged()
    {
        enemyState.ChangeState(EnemyState.State.Chase);

        anim.SetBool("IsDamaged", false);
    }

    IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(0.01f);

        RaycastHit hit;

        if(Physics.Raycast(transform.position, tempPosition - transform.position, out hit, attackRanage, enemyState.layerMask))
        {
            Debug.DrawRay(transform.position, hit.transform.position - transform.position, Color.green, 1.0f);
            if (hit.transform.CompareTag("Player"))
            {
                print("때림!");
                GameManager.instance.Damaged(1);
            }
            else
            {
                enemyState.ChangeState(EnemyState.State.Chase);
            }
        }
    }

    void IsTurn()
    {
        tempPosition = GameManager.instance.player.transform.position;

        Vector3 lookat = new Vector3(tempPosition.x, transform.position.y, tempPosition.z);

        transform.parent.LookAt(lookat);
    }
}
