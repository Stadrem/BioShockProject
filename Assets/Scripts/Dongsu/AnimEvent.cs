using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class AnimEvent : MonoBehaviour
{
    EnemyState enemyState;

    public float attackRanage = 5.0f;

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

        Ray ray = new Ray(attackPoint.transform.position, attackPoint.transform.forward);

        RaycastHit hitInfo;

        if (Physics.SphereCast(ray, 0.1f, out hitInfo, attackRanage, layerMask))
        {
            Debug.DrawRay(attackPoint.transform.position, attackPoint.transform.forward, Color.green);
            if (hitInfo.transform.gameObject.CompareTag("Player"))
            {
                GameManager.instance.Damaged(1);
            }
        }
        else
        {
            enemyState.ChangeState(EnemyState.State.Chase);
        }
        
    }

    void IsDamaged()
    {
        enemyState.ChangeState(EnemyState.State.Chase);

        anim.SetBool("IsDamaged", false);
    }
}
