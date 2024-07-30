using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossBehavior : MonoBehaviour
{
    // 에너미 상태
    public enum EnemyState
    {
        Idle,
        Move,
        Attack,
        Damaged,
        Die
    }
    // 에너미 상태 변수
    public EnemyState state;
    // 플레이어 발견 범위
    public float findDistance = 8f;
    // 플레이어 공격 가능 범위
    public float attackDistance = 2f;
    // Player Transform
    Transform player;
    // 캐릭터 컨트롤러 컴포넌트
    CharacterController cc;
    // 현재 시간
    float currTime = 0;
    // 공격 딜레이 시간
    float attackDelayTime = 2f;
    // 이동 방향 
    Vector3 dir;
    // 보스 공격력
    public int attackPower = 3;
    // 이동속도
    public float moveSpeed = 2;

    // 근접 공격 범위
    public float meleeAttackDistance = 2f;
    // 근접 공격력
    public int meleeAttackPower = 10;
    // 중거리 공격 범위
    public float shotAttackDistance = 4f;
    // 중거리 공격력
    public int shotAttackPower = 5;
    // 보스의 오른손
    public Transform rightHand;
    // 회전할것인가?
    bool isRoatate = false;
    // 회전속도
    public float rotationSpeed = 2f;
    // 회전 후 대기 시간
    public float pauseDuration = 1f;
    // 원래 회전각
    private Quaternion originalRotation;
    // 타겟 회전각
    private Quaternion targetRotation;
    // 플레이어가 가까운가?
    bool isPlayerClose = false;

    // 돌진 속도
    public float chargeSpeed = 10f;
    // 돌진 시작 거리
    public float chargeRange = 15f;
    // 돌진 여부
    public bool isCharging = false;
    // 캐릭터의 동작 여부
    bool isMoving = false;
    // 보스 데미지 스크립트 참조
    private BossDamaged bossDamaged;

    void Start()
    {
        // 최초의 보스 상태는 Idle
        state = EnemyState.Idle;
        // Player의 Transform 컴포넌트 받아오기
        player = GameObject.Find("Player").transform;
        // 빅대디의 캐릭터 컨트롤러 컴포넌트 받아오기
        cc = GetComponent<CharacterController>();
        // 초기 회전 상태 저장
        originalRotation = rightHand.rotation;
        targetRotation = Quaternion.Euler(-50, -47, 54) * originalRotation;
        // 보스 데미지 스크립트
        bossDamaged = GetComponent<BossDamaged>();
    }
     
    void Update()
    {
        // 플레이어가 있는 방향으로 몸을 회전시킨다.
        Vector3 directionToPlayer = player.position - transform.position;
        directionToPlayer.y = 0;
        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);

        // 보간을 이용하여 속도 조절
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 1f);

        switch (state)
        {
            case EnemyState.Idle:
                Idle();
                break;
            case EnemyState.Move:
                Move();
                break;
            case EnemyState.Attack:
                Attack();
                break;
            case EnemyState.Damaged:
                // Damaged 상태에서 특정 행동을 취할 수 있다.
                break;
            case EnemyState.Die:
                Die();
                break;
        }
    }

    // 상태 변경 함수
    public void ChangeState(EnemyState newState)
    {
        state = newState;

        switch(state)
        {
            case EnemyState.Idle:
                // 대기 상태 로직
                break;
            case EnemyState.Move:
                // 이동 상태 로직
                break;
            case EnemyState.Attack:
                // 공격 상태 로직
                break;
            case EnemyState.Damaged:
                // 피해 상태 로직
                break;
            case EnemyState.Die:
                // 2초 후에 오브젝트를 제거시킨다.
                StartCoroutine(RemoveAfterDelay(2.0f));
                // 죽음 상태 로직
                break;
        }
    }

    // 대기 상태 함수
    public void Idle()
    {
        // 플레이어와의 거리가 인지범위 안에 들어오면, Move 상태로 전환한다.
        float dist = Vector3.Distance(player.transform.position, transform.position);
        if (findDistance > dist)
        {
            // move 상태로 변환
            state = EnemyState.Move;
        }
    }

    // 이동 상태 함수
    public void Move()
    {
        // 플레이어와 보스의 거리 구하기
        float dist = Vector3.Distance(player.transform.position, transform.position);

        // 적이 플레이어와의 거리가 근접 공격 범위 이내에 있으면 공격 상태로 전환
        if (dist < meleeAttackDistance)
        {
            state = EnemyState.Attack;
            // 이동 벡터를 0으로 설정하여 이동을 멈춘다.
            cc.Move(Vector3.zero);
            return;
        }

        // 플레이어와의 거리가 공격 범위 밖이라면 플레이어 방향으로 향한다.
        dir = player.transform.position - transform.position;
        dir.Normalize();
        dir.y = 0;
        // 캐릭터 컨트롤러를 이용하여 이동
        cc.Move(dir * moveSpeed * Time.deltaTime);
    }

    // 공격 함수
    // 플레이어가 공격 받았을 때 뒤로 밀리는 효과 추가(플레이어 위치 받아와서 back으로)
    public void Attack()
    {
        // 만약, 플레이어가 공격 범위 이내에 있다면 플레이어를 공격한다.
        float dist = Vector3.Distance(player.transform.position, transform.position);

        // 공격 패턴에 따른 공격 범위 및 처리
        // 근거리
        if (dist < meleeAttackDistance)
        {
            currTime += Time.deltaTime;
            if (currTime >= attackDelayTime)
            {
                // 싱글톤으로 HP 관리
                GameManager.instance.Damaged(attackPower);
                MeleeAttack();
                currTime = 0;
            }
        }
        // 중거리
        else if (dist < shotAttackDistance)
        {
            currTime += Time.deltaTime;
            if (currTime >= attackDelayTime)
            {
                // 싱글톤으로 HP 관리
                GameManager.instance.Damaged(attackPower);
                RandomShotAttack();
                currTime = 0;
            }
        }
        else
        {
            state = EnemyState.Move;
        }
    }
    // 근접 공격
    public void MeleeAttack()
    {
        // 오른팔을 회전시킨다.
        isRoatate = true;
        rightHand.rotation = targetRotation;

        // 플레이어와 멀어지면
        if (isPlayerClose)
        {
            // 오른손을 원래 위치로 한다.
            rightHand.rotation = originalRotation;
        }

        // 오른쪽 팔의 드릴을 이용하여 후려친다. 패턴은 위 대각선, 아래 대각선 2개로 랜덤하게 부여(ani)
        print("근접 공격");
    }

    // 중거리 랜덤 공격
    public void RandomShotAttack()
    {
        // 중거리 공격 2개중 랜덤하게 부여
        int attackType = Random.Range(0, 2);

        if (attackType == 0)
        {
            // 중거리 공격1
            ShotAttackType1();
        }
        else
        {
            // 중거리 공격2
            ShotAttackType2();
        }
    }

    // 중거리 공격1 - 돌진 공격
    public void ShotAttackType1()
    {
        // 플레이어와의 거리 
        float dist = Vector3.Distance(player.transform.position, transform.position);
        // 플레이어와의 거리가 돌진 범위 보다 크다면
        if (dist < chargeRange)
        {
            if (!isCharging)
            {
                print("돌진 공격");
                StartCoroutine(ChargeTowardsPlayer());
            }
        }

        
    }

    // 중거리 공격2 - 전방위 공격(땅내려치기)
    public void ShotAttackType2()
    {
        // 카메라 시야 흐려짐
        print("땅내려치기");
    }

    // 플레이어를 향해 돌진, 일정 시간이 지나면 이동 상태로 돌아간다.
    private IEnumerator ChargeTowardsPlayer()
    {
        // 돌진할것인가
        isCharging = true;
        // 이동속도를 돌진속도로 변환
        this.moveSpeed = chargeSpeed;

        float chargeDutation = 1f;
        float startTime = Time.time;

        while (Time.time < startTime + chargeDutation)
        {
            print("돌진중");
            yield return null;
        }

        this.moveSpeed = moveSpeed;
        isCharging = false;
        state = EnemyState.Move;
    }

    public void Damaged(int damage, string type)
    {
        GetComponent<BossDamaged>().Damaged(damage, type);

        // 동결
        // 감전
        // 근거리 및 원거리 공격
    }

    public void Die()
    {
        print("사망");
        // 캐릭터 컨트롤러 비활성화
        GetComponent<CharacterController>().enabled = false;
        // 애니메이션 트리거 추가
        // if(animoator != null)
        //{
        //    Animator.SetTrigger("Die");
        //}
    }

    private IEnumerator RemoveAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    // 공격시 충돌 처리 
    private void OnTriggerEnter(Collider other)
    {
        // 객체가 Player, 에너미 상태가 attack일 경우
        if(other.CompareTag("Player") && state == EnemyState.Attack)
        {
            // 플레이어에게 피해를 입힌다.
            GameManager.instance.Damaged(attackPower);
        }
    }
}
