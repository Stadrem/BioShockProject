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

    //Animator 가져오기
    Animator anim;

    public Rigidbody[] ragdollRigidbodies;


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
    private State previousState;

    // Start is called before the first frame update
    void Start()
    {
        na = GetComponent<NavMeshAgent>(); // NavMeshAgent 컴포넌트를 가져옴
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        currentState = State.Idle;
        previousState = currentState;

        // 레그돌의 리지드바디를 비활성화
        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.isKinematic = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 상태가 변경되었는지 확인
        if (currentState != previousState)
        {
            OnStateChanged();
            previousState = currentState;
        }
        if (ChaseOn == true)
        {
            ChaseState();
        }
    }

    void OnStateChanged()
    {
        switch (currentState)
        {
            case State.Idle:
                IdleState();
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
                print("데미지");
                DamagedState();
                break;

            case State.Freeze:
                FreezeState();
                break;

            case State.Die:
                DieState();
                break;
        }

    }

    public void ChangeState(State newState)
    {
        currentState = newState;
    }

    void IdleState()
    {
        anim.SetTrigger("IsIdle");
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
        na.isStopped = true;
        anim.speed = 0;
        StartCoroutine(FreezeTime(freezeTime));
    }

    void ChaseState()
    {
        anim.SetBool("IsAttack", false);
        anim.SetBool("IsWalk", true);
        na.isStopped = false;
        na.SetDestination(GameManager.instance.player.transform.position);
    }

    void AttackState()
    {
        ChaseOn = false;
        anim.SetBool("IsAttack", true);
        anim.SetBool("IsWalk", false);
    }

    void DieState()
    {
        na.enabled = false;
        // 애니메이터 비활성화
        anim.enabled = false;

        // 레그돌의 리지드바디 활성화
        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.isKinematic = false;
        }

        AttackRange.SetActive(false);
        ChaseRange.SetActive(false);

        this.enabled = false;
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
        na.isStopped = false;
    }
}