using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.AI;
using static EnemyAttack;
using static UnityEngine.GraphicsBuffer;

public class AnimEvent : MonoBehaviour
{
    EnemyState enemyState;

    NavMeshAgent nav;

    public IAttack attackSC;

    //Animator 가져오기
    Animator anim;

    //타겟 위치 임시 저장
    Vector3 tempPosition;

    private void Start()
    {
        attackSC = GetComponent<IAttack>();

        enemyState = GetComponentInParent<EnemyState>();

        nav = GetComponentInParent<NavMeshAgent>();

        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // 현재 속도 벡터를 얻고, 그 크기를 구합니다.
        float speed = nav.velocity.magnitude;

        // 속도가 0.1 이상일 때 2번째 레이어의 weight 값을 1로 설정합니다.
        if (speed > 0.05f && speed < 6)
        {
            anim.SetLayerWeight(1, 0.5f);
        }
        else if(speed > 6)
        {
            anim.SetLayerWeight(1, 0f);
        }
        else
        {
            anim.SetLayerWeight(1, 0f);
        }

        // Animator의 float 파라미터를 갱신합니다.
        //anim.SetFloat("Speed", speed);
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
        enemyState.dieSound.Play();
        enemyState.WaitStop();
    }
}
