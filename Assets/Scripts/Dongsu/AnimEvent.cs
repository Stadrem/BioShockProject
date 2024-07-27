using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class AnimEvent : MonoBehaviour
{
    EnemyState enemyState;

    public float attackRanage = 7.0f;

    public LayerMask layerMask;

    public GameObject attackPoint;



    //Animator 가져오기
    Animator anim;

    private void Start()
    {
        enemyState = GetComponentInParent<EnemyState>();
        anim = GetComponent<Animator>();
    }

    void IsMeleeAttack()
    {
        float distance = Vector3.Distance(GameManager.instance.player.transform.position, transform.position);

        if (distance > enemyState.reAttackDistance)
        {
            enemyState.ChangeState(EnemyState.State.Chase);
        }

        Collider[] hitColliders = Physics.OverlapSphere(attackPoint.transform.position, 2f, layerMask);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                print("때림!");
                GameManager.instance.Damaged(1);
                break;
            }
            else
            {
                enemyState.ChangeState(EnemyState.State.Chase);
                print("없는데요?");
            }
        }
    }

    void IsDamaged()
    {
        enemyState.ChangeState(EnemyState.State.Chase);

        anim.SetBool("IsDamaged", false);
    }
}
