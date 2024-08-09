using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ShotAttackDecide : MonoBehaviour
{
    private BossBehavior bossBehavior;
    Transform player;
    NavMeshAgent agent;

    float moveSpeed = 2f;
    float chargeSpeed = 10f;

    // Start is called before the first frame update
    void Start()
    {
        // 보스 행동 스크립트 처리
        bossBehavior = GetComponent<BossBehavior>();
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    public void Update()
    {
        //Attack();
    }

    //public void Attack()
    //{
    //    // 거리 측정
    //    float dist = Vector3.Distance(player.position, transform.position);
    //    if (dist > 5)
    //    {
    //        // 속도
    //        agent.speed = moveSpeed;
    //        // 돌진
    //        bossBehavior.ChangeState(BossBehavior.EnemyState.ShotAttack);
    //    }
    //    else
    //    {
    //        // 속도
    //        agent.speed = chargeSpeed;
    //        // 상태 변화
    //        bossBehavior.ChangeState(BossBehavior.EnemyState.Idle);
    //        // 돌진
    //    }
    //}

    public void ChargeAttack()
    {
        //거리 측정
        float dist = Vector3.Distance(player.position, transform.position);
        if (dist > 5)
        {
            // 속도
            agent.speed = moveSpeed;
            // 돌진
            bossBehavior.ChangeState(BossBehavior.EnemyState.ShotAttackType1);
        }
        else
        {
            // 속도
            agent.speed = chargeSpeed;
            // 상태 변화
            bossBehavior.ChangeState(BossBehavior.EnemyState.Idle);
        }


        
    }
}
