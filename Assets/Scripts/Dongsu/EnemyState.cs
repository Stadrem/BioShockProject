using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyState : MonoBehaviour
{
    //public float speed = 10.0f;

    Rigidbody rb;

    //Vector3 dir;

    bool ChaseOn = false;

    NavMeshAgent na;

    public enum State
    {
        Idle,
        Patroll,
        Chase,
        Attack,
        Stun,
        Freeze,
        Damaged,
        Die
    }

    public GameObject AttackRange;
    public GameObject ChaseRange;

    public State currentState;

    // Start is called before the first frame update
    void Start()
    {
        na = GetComponent<NavMeshAgent>(); // NavMeshAgent 컴포넌트를 가져옴
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case State.Idle:
                break;

            case State.Patroll:
                break;

            case State.Chase:
                //transform.LookAt(GameManager.instance.player.transform);
                ChaseOn = true;
                break;

            case State.Attack:
                break;

            case State.Stun:
                break;

            case State.Damaged:
                break;

            case State.Die:
                break;
        }

        if (ChaseOn == true)
        {
            ChaseState();
        }
    }
    
    public void ChangeState(string newState)
    {
        currentState = (State)Enum.Parse(typeof(State), newState);
    }

    void ChaseState()
    {
        //dir = GameManager.instance.player.transform.position - transform.position;

        //dir.Normalize();

        na.SetDestination(GameManager.instance.player.transform.position);
        //rb.velocity = dir * speed;
    }
}
