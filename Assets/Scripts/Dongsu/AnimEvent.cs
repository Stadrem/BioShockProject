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
        Ray ray = new Ray(attackPoint.transform.position, attackPoint.transform.forward);

        RaycastHit hitInfo;

        if (Physics.SphereCast(ray, 1.0f, out hitInfo, attackRanage, layerMask))
        {
            if (hitInfo.transform.gameObject.CompareTag("Player"))
            {
                print("인식완료");

                GameManager.instance.Damaged(1);
            }
            else
            {
                print("인식불가");
            }
        }
    }

    void IsDamaged()
    {
        enemyState.ChangeState(EnemyState.State.Chase);
        anim.SetBool("IsDamaged", false);
    }
}
