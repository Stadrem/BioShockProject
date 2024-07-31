using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEditor.PackageManager;
using UnityEngine;
using static EnemyAttack;
using static UnityEngine.GraphicsBuffer;

public class AnimEvent : MonoBehaviour
{
    EnemyState enemyState;

    public GameObject attackPoint;

    public IAttack attackSC;

    //Animator 가져오기
    Animator anim;

    //타겟 위치 임시 저장
    Vector3 tempPosition;

    private void Start()
    {
        attackSC = GetComponent<IAttack>();

        enemyState = GetComponentInParent<EnemyState>();

        anim = GetComponent<Animator>();
    }

    void IsAttack()
    {
        attackSC.Attack();
    }

    void IsDamaged()
    {
        anim.SetBool("IsDamaged", false);
    }

    void IsTurn()
    {
        tempPosition = GameManager.instance.player.transform.position;

        Vector3 lookat = new Vector3(tempPosition.x, transform.position.y, tempPosition.z);

        transform.parent.LookAt(lookat);
    }

    void IsStun()
    {
        enemyState.WaitStop();
    }
}
