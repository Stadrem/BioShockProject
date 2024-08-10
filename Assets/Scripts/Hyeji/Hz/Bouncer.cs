using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Bouncer : MonoBehaviour
{
    // 파티클 시스템 오브젝트
    GameObject particlesRing;
    // 에너미 상태
    public enum EnemyState
    {
        Idle,
        Move,
        Melee,
        GroundSlam,
        ChargeAttack,
        Damaged,
        Die  
    }

    public EnemyState currentState;
    // 대기 시간
    public float idleTime = 5f;
    // 공격 딜레이 타임
    public float attackDelayTime = 2f;
    // 근접 공격 거리
    public float meleeAttackDistance = 2f;
    // 땅 내려치기 공격
    public float groundSlamDistance = 10f;
    // 돌진 공격
    public float chargeAttackDistance = 10f;
    // 근접 공격력
    public int meleeAttackPower = 10;
    // 땅 내려치기 공격력
    public int groundSlamPower = 15;
    // 돌진 공격력
    public int chargeAttackPower = 20;

    public int attackPower = 3;

    // 돌진 속도
    public float chargeSpeed = 10f;

    // 네뷔
    private NavMeshAgent agent;
    // 애니메이터
    private Animator anim;
    // 플레이어 위치
    private Transform player;
    // 현재 시간
    private float currTime = 0;
    // 공격중이니?
    private bool isAttacking = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        player = GameObject.FindWithTag("Player").transform;

        ChangeState(EnemyState.Idle);
    }

    bool isKnockback;
    float knockbackTime = 0.2f;

    void Update()
    {
        LookAtPlayer();

        // 넉백 처리
        if (isKnockback == true)
        {
            currTime += Time.deltaTime;
            GameManager.instance.player.transform.position += Vector3.back * 50 * Time.deltaTime;

            if (currTime > knockbackTime)
            {
                isKnockback = false;
                currTime = 0;
            }
        }


        switch (currentState)
        {
            case EnemyState.Idle:
                Idle();
                break;
            case EnemyState.Move:
                if(!isAttacking)
                {
                    Move();
                }    
                break;
            case EnemyState.Melee:
                MeleeAttack();
                break;
            case EnemyState.GroundSlam:
                GroundSlamAttack();
                break;
            case EnemyState.ChargeAttack:
                ChargeAttack();
                break;
            case EnemyState.Damaged:
                // Damaged 상태 로직 추가
                break;
            case EnemyState.Die:
                Die();
                break;
        }
    }

    void ChangeState(EnemyState newState)
    {
        if (currentState == newState)
            return;

        Debug.Log($"상태 변경: {currentState} -> {newState}");

        currentState = newState;
        agent.isStopped = true;

        switch (newState)
        {
            case EnemyState.Idle:
                anim.SetTrigger("Idle");
                currTime = 0; // Idle 상태 진입 시 타이머 초기화
                break;
            case EnemyState.Move:
                // 내가 추가
                // 공격 상태라면
                if(!isAttacking)
                {
                    agent.isStopped = false;
                    anim.SetTrigger("Move");
                    Debug.Log("Move 상태 진입");
                }
                break;
            case EnemyState.Melee:
                agent.isStopped = true;
                anim.SetTrigger("Melee");
                // 추가함내가
                //isAttacking = true;
                StartCoroutine(PerformMeleeAttack());
                break;
            case EnemyState.GroundSlam:
                agent.isStopped = true;
                anim.SetTrigger("Shot2");
                // 추가함내가
                isAttacking = true;
                StartCoroutine(PerformGroundSlamAttack());
                break;
            case EnemyState.ChargeAttack:
                agent.isStopped = true;
                anim.SetTrigger("Shot");
                // 추가함내가
                isAttacking = true;
                StartCoroutine(PerformChargeAttack());
                break;
            case EnemyState.Damaged:
                agent.isStopped = true;
                anim.SetTrigger("Damage");
                break;
            case EnemyState.Die:
                {
                    agent.isStopped = true;
                    // 2초 후에 오브젝트를 제거시킨다.
                    StartCoroutine(RemoveAfterDelay(2.0f));
                    anim.SetTrigger("Die");
                }
                break;
        }
    }

    void LookAtPlayer()
    {
        // 보스가 플레이어의 위치를 바라보도록 회전
        Vector3 direction = player.position - transform.position;
        direction.y = 0; // 수평 축만 고려하기 위해 y축 제거
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    void Idle()
    {
        currTime += Time.deltaTime;
        if (currTime >= idleTime)
        {
            ChangeState(EnemyState.Move);
        }
    }

    void Move()
    {
        // 공격상태가 아니거나, 현재 상태가 움직이는 상태가 아닐경우
        if (isAttacking || currentState != EnemyState.Move)
            return;

        float dist = Vector3.Distance(player.position, transform.position);

        if (dist <= meleeAttackDistance)
        {
            ChangeState(EnemyState.Melee);
        }
        else if (dist > meleeAttackDistance && dist <= groundSlamDistance)
        {
            ChangeState(EnemyState.GroundSlam);
        }
        else if (dist > groundSlamDistance)
        {
            ChangeState(EnemyState.ChargeAttack);
        }
        else
        {
            agent.SetDestination(player.position);
        }
    }


    IEnumerator PerformMeleeAttack()
    {
        if (currentState != EnemyState.Melee)
            yield break;

        isAttacking = true;
        yield return new WaitForSeconds(attackDelayTime);
        if (currentState == EnemyState.Melee)
        {
            GameManager.instance.Damaged(meleeAttackPower);
            ChangeState(EnemyState.Move);
        }
        isAttacking = false;
    }

    void MeleeAttack()
    {
        if (!isAttacking)
        {
            StartCoroutine(PerformMeleeAttack());
        }
    }

    IEnumerator PerformGroundSlamAttack()
    {
        if (currentState != EnemyState.GroundSlam)
            yield break;

        Debug.Log("GroundSlam 시작");

        isAttacking = true;

        yield return new WaitForSeconds(attackDelayTime);

        if (currentState == EnemyState.GroundSlam)
        {
            GameManager.instance.Damaged(groundSlamPower);

            Debug.Log("GroundSlam 공격 완료");
        }

        anim.ResetTrigger("Shot2");

        Debug.Log("GroundSlam 종료, Move 상태로 전환 시도");

        isAttacking = false;
        ChangeState(EnemyState.Move);
        

        Debug.Log("GroundSlam 종료");

    }

    void GroundSlamAttack()
    {
        if (!isAttacking)
        {
            StartCoroutine(PerformGroundSlamAttack());
        }
    }

    IEnumerator PerformChargeAttack()
    {
        if (currentState != EnemyState.ChargeAttack)
            yield break;

        Debug.Log("ChargeAttack 시작");

        float originalSpeed = agent.speed;
        agent.speed = chargeSpeed;

        // 돌진 시작 시 플레이어의 위치를 고정
        Vector3 tempPosition = player.position;

        // 돌진 동안 이동
        float chargeDuration = 2f; // 돌진 지속 시간
        float startTime = Time.time;

        while (Time.time < startTime + chargeDuration)
        {
            if (currentState != EnemyState.ChargeAttack)
                yield break;

            // 고정된 위치로 돌진
            agent.SetDestination(tempPosition);
            yield return null;
        }

        agent.speed = originalSpeed;
        Debug.Log("ChargeAttack 종료, Move 상태로 전환 시도");
        ChangeState(EnemyState.Move);
        isAttacking = false;

        Debug.Log("ChargeAttack 종료 후 isAttacking 해제");
    }

    void ChargeAttack()
    {
        if (!isAttacking)
        {
            StartCoroutine(PerformChargeAttack());
        }
    }

    public void Damaged(int damage, string type)
    {
        anim.SetTrigger("Damage");
        GetComponent<BossDamaged>().Damaged(damage, type);

    }

    public void Die()
    {
        anim.SetTrigger("Die");
        print("사망");

    }

    private IEnumerator RemoveAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    // 파티클 라이징 생성 함수
    void ParticleMake()
    {
        // 파티클 라이징 생성시킨다.
        GameObject rising = Instantiate(particlesRing);
        // 파티클 라이징의 위치를 빅대디의 위치로 한다.
        rising.transform.position = transform.position;
        // 파티클 시스템 컴포넌트 가져오기
        ParticleSystem ps = rising.GetComponent<ParticleSystem>();
        // 컴포넌트 있으면 실행하게 하기
        if (ps != null)
        {
            ps.Play();
        }
        // 2초가 지나면 파괴하게 하기
        Destroy(rising, 2);
    }

    // 공격시 충돌 처리 
    private void OnTriggerEnter(Collider other)
    {
        print(currentState);

        // 공격 상태일 때 파티클 생성
        if (currentState == EnemyState.Melee ||
        currentState == EnemyState.GroundSlam ||
        currentState == EnemyState.ChargeAttack)

        {
            // 부딪히면 파티클 생성
            //ParticleMake();

            // 맞은 대상이 플레이어라면
            if (other.CompareTag("Player"))
            {
                print("피해입히기");

                int damage = 0;

                // 현재 상태에 따른 피해량 결정
                switch (currentState)
                {
                    case EnemyState.Melee:
                        damage = meleeAttackPower;
                        break;
                    case EnemyState.GroundSlam:
                        damage = groundSlamPower;
                        break;
                    case EnemyState.ChargeAttack:
                        damage = chargeAttackPower;
                        break;
                }

                // 플레이어에게 피해를 입힌다.
                GameManager.instance.Damaged(attackPower);
            }
        }
    }
}
