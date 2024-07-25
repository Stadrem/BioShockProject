using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyState : MonoBehaviour
{
    public float shockStunTime = 2.0f;
    public float freezeTime = 6.0f;

    Rigidbody rb;

    bool ChaseOn = false;

    NavMeshAgent na;
    float tempSpeed = 0;

    //Animator 가져오기
    Animator anim;


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
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case State.Idle:
                IdleState();
                break;

            case State.Patroll:
                PatrollState();
                break;

            case State.Chase:
                ChaseOn = true;
                break;

            case State.Attack:
                AttackState();
                break;

            case State.Stun:
                StunState();
                break;

            case State.Damaged:
                DamagedState();
                break;

            case State.Freeze:
                FreezeState();
                break;

            case State.Die:
                DieState();
                break;
        }

        if (ChaseOn == true)
        {
            ChaseState();
        }
    }
    
    public void ChangeState(State newState)
    {
        currentState = newState;
    }

    void IdleState()
    {
        //anim.SetTrigger("IsIdle");
    }
    void PatrollState()
    {
        anim.SetTrigger("IsWalk");
    }

    void StunState()
    {
        StartCoroutine(StunTime(shockStunTime));
    }

    void DamagedState()
    {
        anim.SetBool("IsDamaged", true);
    }

    void FreezeState()
    {
        tempSpeed = na.speed;
        na.speed = 0;
        anim.speed = 0;
        StartCoroutine(FreezeTime(freezeTime));
    }

    void ChaseState()
    {
        na.SetDestination(GameManager.instance.player.transform.position);
    }

    void AttackState()
    {
        ChaseOn = false;
        anim.SetTrigger("IsAttack");
    }

    void DieState()
    {
        na.enabled = false;
        anim.SetBool("IsDie", true);
    }

    //스턴 시간 코루틴
    IEnumerator StunTime(float time)
    {
        anim.SetBool("IsStun", true);

        yield return new WaitForSeconds(time);

        anim.SetBool("IsStun", false);
    }

    //프리즈 시간 코루틴
    IEnumerator FreezeTime(float time)
    {
        yield return new WaitForSeconds(time);

        anim.speed = 1;

        na.speed = tempSpeed;
    }
}
