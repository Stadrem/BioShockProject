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

    private void Start()
    {
        enemyState = GetComponentInParent<EnemyState>();
        anim = GetComponent<Animator>();
    }

    void IsAttack()
    {
        Vector3 tempPosition = GameManager.instance.player.transform.position;

        float distance = Vector3.Distance(tempPosition, transform.position);

        //상대와 나의 거리가 reAttackDistance보다 크면, 추적
        if (distance > enemyState.reAttackDistance)
        {
            enemyState.ChangeState(EnemyState.State.Chase);
        }

        StartCoroutine(AttackDelay(tempPosition));
    }

    void IsDamaged()
    {
        enemyState.ChangeState(EnemyState.State.Chase);

        anim.SetBool("IsDamaged", false);
    }

    IEnumerator AttackDelay(Vector3 temp)
    {
        yield return new WaitForSeconds(0.4f);

        RaycastHit hit;

        if(Physics.Raycast(transform.position, temp - transform.position, out hit, attackRanage, enemyState.layerMask))
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
        Vector3 lookat = new Vector3(GameManager.instance.player.transform.position.x, transform.position.y, GameManager.instance.player.transform.position.z);

        transform.parent.LookAt(lookat);
    }
}
