using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyState : MonoBehaviour
{
    public LayerMask layerMask;

    public string enemyName;
    public float shockStunTime = 2.0f;
    public float freezeTime = 6.0f;
    public float reAttackDistance = 4.5f;
    public float baseSpeed = 4;

    Rigidbody rb;

    bool ChaseOn = false;

    NavMeshAgent na;

    //Animator 가져오기
    Animator anim;

    public Rigidbody[] ragdollRigidbodies;

    bool firstHit = true;

    //bool dying = false;

    public GameObject AttackRange;
    public GameObject ChaseRange;
    public State currentState;
    private State previousState;
    public float alertRadius = 10.0f;
    public GameObject itemBox;

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

    // Start is called before the first frame update
    void Start()
    {
        itemBox.SetActive(false);
        na = GetComponent<NavMeshAgent>(); // NavMeshAgent 컴포넌트를 가져옴
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        currentState = State.Idle;
        previousState = currentState;
        na.speed = baseSpeed;

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
        na.velocity = Vector3.zero;
        anim.SetBool("IsDamaged", true);
    }

    void FreezeState()
    {
        StartCoroutine(FreezeTime(freezeTime));
    }

    void ChaseState()
    {
        if(firstHit == true)
        {
            anim.SetTrigger("IsFirstDetect");
            StartCoroutine(Scream());
        }
        else
        {
            na.speed = baseSpeed;
            anim.SetBool("IsAttack", false);
            anim.SetBool("IsWalk", true);

            if (na.isActiveAndEnabled)
            {
                na.SetDestination(GameManager.instance.player.transform.position);
            }

            float distance = Vector3.Distance(GameManager.instance.player.transform.position, transform.position);

            //상대와 나의 거리가 reAttackDistance보다 작으면 공격
            if (distance <= reAttackDistance)
            {
                ChangeState(EnemyState.State.Attack);
            }
        }
    }

    void AttackState()
    {
        na.speed = 0;
        ChaseOn = false;
        anim.SetBool("IsAttack", true);
        anim.SetBool("IsWalk", false);

    }

    void DieState()
    {
        // 레그돌의 리지드바디 활성화
        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.isKinematic = false;
        }

        itemBox.SetActive(true);

        this.enabled = false;

        AttackRange.SetActive(false);

        ChaseRange.SetActive(false);

        na.ResetPath();
        na.enabled = false;

        // 애니메이터 비활성화
        anim.enabled = false;
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
        anim.speed = 0;

        yield return new WaitForSeconds(time);

        anim.speed = 1;
    }

    void AlertNearbyEnemies()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, alertRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                EnemyState enemy = hitCollider.GetComponent<EnemyState>();

                if (enemy != null && enemy != this && enemy.firstHit == true)
                {
                    enemy.ChangeState(EnemyState.State.Chase);
                }
            }
        }
    }

    IEnumerator Scream()
    {
        //플레이어 방향 바라보기
        Vector3 lookPos = GameManager.instance.player.transform.position - transform.position;

        // Y축 회전을 방지
        lookPos.y = 0; 

        Quaternion rotation = Quaternion.LookRotation(lookPos);

        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 10);

        yield return new WaitForSeconds(1);

        firstHit = false;

        AlertNearbyEnemies();

        ChaseState();
    }
}