using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnMeleeRay : MonoBehaviour
{
    // 보스행동 스크립트
    private BossBehavior bossBehavior;
    // 오디오 소스
    public AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        // bossBehavior 스크립트 참조
        bossBehavior = GetComponentInParent<BossBehavior>();
        // Audio
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        //Meleeray();
    }

    public void Meleeray()
    {
        // "Player" 레이어를 playerLayer로 설정
        int playerLayer = LayerMask.GetMask("Player");

        Debug.Log("MeleeRay 호출됨");

        // 플레이어의 중심을 향하는 방향 벡터를 계산
        Vector3 origin = bossBehavior.rayOrigin.position;
        Vector3 direction = (bossBehavior.player.position - origin).normalized;

        // 현재 보스와 플레이어의 거리 확인
        float distanceToPlayer = Vector3.Distance(bossBehavior.player.position, origin);
        Debug.Log("보스와 플레이어 간 거리: " + distanceToPlayer);

        // 레이캐스트 발사
        RaycastHit hit;

        Debug.DrawRay(origin, direction * 12, Color.red, 1.0f);

        if (Physics.Raycast(origin, direction, out hit, 7, playerLayer))
        {
            Debug.Log("RayCast 충돌 발생");

            if (hit.transform.CompareTag("Player"))
            {
                Debug.Log("플레이어와 충돌");

                // 부딪히면 파티클 생성
                bossBehavior.ParticleMake();

                // 넉백 적용
                bossBehavior.isKnockback = true;

                // 전기 사운드 소리
                if (bossBehavior.collisionSound != null && bossBehavior.audioSource != null)
                {
                    bossBehavior.audioSource.PlayOneShot(bossBehavior.collisionSound);
                    Debug.Log("전기파지직");                   
                }
                
                // 데미지 전달
                GameManager.instance.Damaged(bossBehavior.meleeAttackPower);
            }
        }
        else
        {
            // 플레이어와 보스의 거리 구하기
            float dist = Vector3.Distance(bossBehavior.player.transform.position, transform.position);
            if (dist > bossBehavior.meleeAttackDistance)
            {

                bossBehavior.ChangeState(BossBehavior.EnemyState.ShotAttack);
                //ChangeState(EnemyState.Move);
            }
            Debug.Log("충돌없어");
        }
    }

    void Slam()
    {
        print("되는거니?");
        bossBehavior.Shot2Particle();
    }
}
