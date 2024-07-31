using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossBehavior_2 : MonoBehaviour
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
    EnemyState state;
    // 캐릭터 컨트롤러 컴포넌트
    CharacterController cc;
    // player 의 transform
    Transform player;
    // 총구 위치
    public GameObject firePos;
    // 총알 공장
    public GameObject bulletFactory;

    // 플레이어 공격 가능 범위
    public float attackDistance = 2f;
    // 공격 딜레이 시간
    float attackDelayTime = 2f;
    // 보스 공격력
    public int attackPower = 3;
    // 로지 이동 속도
    public float moveSpeed = 2;
    // 무작위 이동 반경
    public float wanderRadius = 10f;
    // 무작위 이동 대기 시간
    public float wanderTimer = 2f;
    // 공격 쿨다운
    public float attackCooldown = 2f;

    // 보스 데미지 스크립트
    private BossDamaged bossDamaged;

    // NavMeshAgent 컴포넌트
    private NavMeshAgent agent;

    private float lastAttackTime;


    // Start is called before the first frame update
    void Start()
    {
        // 최초 보스의 상태 Idle
        state = EnemyState.Idle;
        // 플레이어의 transform 가져오기
        player = GameObject.Find("Player").transform;
        // 캐릭터 컨트롤러 컴포넌트
        cc = GetComponent<CharacterController>();
        // 보스 데미지 스크립트
        bossDamaged = GetComponent<BossDamaged>();
        // NavMeshAgent 컴포넌트
        agent = GetComponent<NavMeshAgent>();

    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case EnemyState.Idle:
                Idle();
                break;
            case EnemyState.Move:
                Move();
                break;
            case EnemyState.Attack:
                Attack(player.transform.position);
                break;
            case EnemyState.Damaged:
                // Damaged 상태에서 특정 행동을 취할 수 있다.
                break;
            case EnemyState.Die:
                Die();
                break;
        }
    }

    void Idle()
    {
        // 보스가 일정한 구역을 자유롭게 돌아다닌다.
        // 플레이어가 인지 거리 범위 안에 들어오면 move 상태로 전환한다.
    }

    void Move()
    {
        // 플레이어와 보스의 거리 구하기
        float dist = Vector3.Distance(player.transform.position, transform.position);

        // 플레이어와의 거리가 공격 범위 내에 있으면
        if(dist < attackDistance)
        {
            // 공격 상태로 변환한다.
            state = EnemyState.Attack;

            // 플레이어를 추적
            if (player != null)
            {
                // 플레이어의 위치로 지정
                agent.SetDestination(player.position);
            }
        }

    }

    void Attack(Vector3 aimPos)
    {
        float dist = Vector3.Distance(player.transform.position, transform.position);

        // 플레이어를 공격, 쿨다운이 지나야 공격 가능
        if(player != null && Time.time > lastAttackTime + attackCooldown)
        {
            // 공격이 가능하다면 lastAttackTime 을 현재시간으로 업데이트한다.
            lastAttackTime = Time.time;

            // 공격 로직 추가 (Raycast)
            Ray ray = new Ray(firePos.transform.position, aimPos - firePos.transform.position);
            RaycastHit hitInfo = new RaycastHit();

            if(Physics.Raycast(ray, out hitInfo))
            {
                // 총알을 생성
                GameObject bullet = Instantiate(bulletFactory);
                bullet.transform.position = firePos.transform.position;
                // 맞은 위치에 두기
                bullet.transform.position = hitInfo.point; 
                // 만든 파편효과의 앞방향을 맞은 위치의 normal 값으로 셋팅한다.
                bullet.transform.forward = hitInfo.normal;
                // 만든 파편효과를 3초뒤에 파괴하자
                Destroy(bullet, 2);

                // 맞은 대상이 Player라면
                if (hitInfo.collider.CompareTag("Player"))
                {
                    // Player에게 데미지를 주자
                    GameManager.instance.Damaged(attackPower);
                }
            }
        }
    }
    
    void Damaged(int damage, string type)
    {
        GetComponent<BossDamaged>().Damaged(damage, type);

        // 동결
        // 감전
        // 근거리 및 원거리 공격
    }

    void Die()
    {
        // 네비게이션 시스템 비활성화
        agent.enabled = false;
        // 캐릭터 컨트롤러 비활성화
        GetComponent<CharacterController>().enabled = false;
        print("죽었다");
        // 사망 애니메이션 추가
    }
}
