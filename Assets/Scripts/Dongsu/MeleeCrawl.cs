using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeleeCrawl : MonoBehaviour
{
    EnemyState enemyState;

    Animator anim;

    NavMeshAgent na;

    // Start is called before the first frame update
    void Start()
    {
        enemyState = GetComponentInParent<EnemyState>();

        anim = GetComponent<Animator>();

        na = GetComponentInParent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if(enemyState.currentState == EnemyState.State.Chase)
        {
            float distance = Vector3.Distance(GameManager.instance.player.transform.position, transform.position);

            //상대와 나의 거리가 15 이상이면 돌진
            if (distance > 5)
            {
                enemyState.baseSpeed = 10;
                anim.SetBool("IsCrawl", true);
            }
            else
            {
                enemyState.baseSpeed = 5;
                anim.SetBool("IsCrawl", false);
            }
        }
    }
}
