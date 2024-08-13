using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.UI;

public class BossDamaged : MonoBehaviour
{
    // 최대 HP
    public int maxHP = 100;
    // 현재 HP
    public int currHP;
    // 피격 횟수
    public int angry = 2;

    // 현재 시간
    public float currTime;

    // HP UI
    //public Slider hpUI;

    // 보스행동 스크립트
    //private BossBehavior bossBehavior;
    //// 보스행동 스크립트2
    //private rosieBehavior rosie;

    // Delegate
    public Action<BossBehavior.EnemyState> onChangeState;
    //public delegate void DeathEventHandler();
    //public static event DeathEventHandler onDeath();

    // Particle System
    ParticleSystem ps;
    // 파티클 오브젝트
    public GameObject ParitcleLight;

    // AudioSource
    private AudioSource audioSource;
    // 사운드 - 빅대디 데미지 상태
    public AudioClip damageSound;


    // Start is called before the first frame update
    void Start()
    {
        // bossBehavior 스크립트 참조
        //bossBehavior = GetComponent<BossBehavior>();
        // rosieBehavior 스크립트 참조
        //rosie = GetComponent<rosieBehavior>();
        // 현재 HP를 최대 HP로 설정하자
        currHP = maxHP;
        // Audio
        audioSource = GetComponent<AudioSource>();
     
    }

    // Update is called once per frame
    void Update()
    {
        // 피격 횟수에 따른 애니메이션 딜레이 적용
        currTime += Time.deltaTime;
        // 리셋시간보다 현재시간이 커지면
        if (currTime > 7)
        {
            // 피격 횟수 초기화
            angry = 0;
            // 현재 시간 초기화
            currTime = 0;
        }
    }

    void MakeParticle()
    {
        GameObject psLight = Instantiate(ParitcleLight);
        psLight.transform.position = transform.position;
        // 파티클 시스템 컴포넌트 가져오기
        ParticleSystem ps = psLight.GetComponent<ParticleSystem>();
        // 컴포넌트 있으면 실행하게 하기
        if (ps != null)
        {
            ps.Play();
        }
        // 2초가 지나면 파괴하게 하기
        Destroy(psLight, 2);
    }

    public void Damaged(int damage, string type)
    {
        // 체력이 0 이하인지 확인
        if (currHP <= 0)
        {
            currHP = 0;
            //bossBehavior.ChangeState(BossBehavior.EnemyState.Die);
            onChangeState(BossBehavior.EnemyState.Die);
            CheckIfDead();
            return;
        }

        // 피격 증가 및 초기화
        angry++;
        if (angry < 5)
        {
            onChangeState(BossBehavior.EnemyState.Damaged);
            print("앵그리확인");
        }


        // HP 바를 갱신하자.
        //float ratio = currHP * 0.01f;
        //hpUI.value = ratio;

        switch (type)
        {
            case "Shock":
                PlayDamageEffect(type);
                StartCoroutine(StunDamageStep(damage, 1.0f));
                break;
            case "Fire":
                PlayDamageEffect(type);
                StartCoroutine(DamageStep(damage, 5, type));
                break;
            case "Ice":
                PlayDamageEffect(type);
                StartCoroutine(FreezeDamageStep(3, 3.0f));
                break;
            default:
                PlayDamageEffect(type);
                StartCoroutine(DamageStep(damage, 1, type));
                break;
        }
        CheckIfDead();
    }
    // 사망 유무 판단 함수
    private void CheckIfDead()
    {
        // 적이 죽었는지 확인한다.
        if (currHP <= 0)
        {
            //bossBehavior.ChangeState(BossBehavior.EnemyState.Die);
            onChangeState(BossBehavior.EnemyState.Die);
        }
        else
        {
            //bossBehavior.ChangeState(BossBehavior.EnemyState.Damaged);
            onChangeState(BossBehavior.EnemyState.Damaged);
        }
    }
    //  동결 상태
    IEnumerator FreezeDamageStep(int damage, float freezeDuration)
    {
        currHP -= damage; //동결 상태에서 데미지 처리 (값은 필요에 따라 조정)
        print("동결 상태");

        // 동결 상태 적용
        //bossBehavior.ChangeState(BossBehavior.EnemyState.Damaged);
        onChangeState(BossBehavior.EnemyState.Damaged);

        // 이동 멈춤
        // 애니메이션 멈춤

        yield return new WaitForSeconds(freezeDuration);

        // 동결 상태 해제
        // 이동 재시작
        // 애니메이션 시작

        // 대기 상태로 전환
        //bossBehavior.ChangeState(BossBehavior.EnemyState.Idle);
        onChangeState(BossBehavior.EnemyState.Idle);

    }

    // 감전 상태
    IEnumerator StunDamageStep(int damage, float stunDuration)
    {

        currHP -= damage;
        print("감전");

        // 스턴 상태 적용
        //bossBehavior.ChangeState(BossBehavior.EnemyState.Damaged);
        onChangeState(BossBehavior.EnemyState.Damaged);

        // 애니메이션 삽입

        yield return new WaitForSeconds(stunDuration);

        // 애니메이션 삽입
        // 대기 상태 변환
        //bossBehavior.ChangeState(BossBehavior.EnemyState.Idle);
        onChangeState(BossBehavior.EnemyState.Idle);

    }


    // 근접, 원거리 공격 상태
    // 근접 피해량 2배 증가
    IEnumerator DamageStep(int damage, int j, string type)
    {
        //bossBehavior.ChangeState(BossBehavior.EnemyState.Damaged);
        onChangeState(BossBehavior.EnemyState.Damaged);

        {
            for (int i = 0; i < j; i++)
            {
                // 근접 공격일 경우 피해량 2배 증가
                if(type == "Melee")
                {
                    currHP -= damage * 2;
                }
                else
                {
                    // 아닐 시, 데미지 감소
                    currHP -= damage;
                }
                if (currHP <= 0)
                {
                    //bossBehavior.ChangeState(BossBehavior.EnemyState.Die);
                    onChangeState(BossBehavior.EnemyState.Die);

                    yield break;
                }
                yield return new WaitForSeconds(0.5f);
            }

            // 대기 상태로 전환한다.
            //bossBehavior.ChangeState(BossBehavior.EnemyState.Idle);
            if(currHP > 0)
            {
                // 이거 계속 사망 상태 이후에 Idle로 잘못전환되는걸 방지함
                onChangeState(BossBehavior.EnemyState.Idle);
            }
            

        }
    }

    // 공통된 피해 효과를 처리
    private void PlayDamageEffect(string type)
    {
        MakeParticle();
        if (damageSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(damageSound);
            Debug.Log($"{type} 타입의 데미지 발생");
        }
    }
}
