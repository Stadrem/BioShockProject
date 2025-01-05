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


    // Delegate
    public Action<BossBehavior.EnemyState> onChangeState;
    //public delegate void DeathEventHandler();
    //public static event DeathEventHandler onDeath();

    // Particle System
    ParticleSystem ps;
    // 파티클 오브젝트
    public GameObject ParticleLight;

    // AudioSource
    private AudioSource audioSource;
    // 사운드 - 빅대디 데미지 상태
    public AudioClip damageSound;


    void Start()
    {
        // 현재 HP를 최대 HP로 설정하자
        currHP = maxHP;
        // Audio
        audioSource = GetComponent<AudioSource>();
        ps = GetComponent<ParticleSystem>();
     
    }

    void MakeParticle()
    {
        GameObject psLight = Instantiate(ParticleLight);
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
            default:
                PlayDamageEffect(type);
                StartCoroutine(DamageStep(damage, 1, type));
                break;
        }
    }

    // 사망 유무 판단 함수
    private void CheckIfDead()
    {
        // 적이 죽었는지 확인한다.
        if (currHP <= 0)
        {
            // Die
            onChangeState(BossBehavior.EnemyState.Die);
        }
        else
        {
            // Damaged
            onChangeState(BossBehavior.EnemyState.Damaged);
        }
    }

    // 감전 상태
    IEnumerator StunDamageStep(int damage, float stunDuration)
    {

        currHP -= damage;
        //print("감전");

        // 스턴 상태 적용
        onChangeState(BossBehavior.EnemyState.Damaged);

        yield return new WaitForSeconds(stunDuration);

        // 대기 상태 변환
        onChangeState(BossBehavior.EnemyState.Idle);

    }

    // 근접, 원거리 공격 상태
    // 근접 피해량 2배 증가
    IEnumerator DamageStep(int damage, int j, string type)
    {
        {
            for (int i = 0; i < j; i++)
            {
                // 아닐 시, 데미지 감소
                currHP -= damage;

                if (currHP <= 0)
                {
                    onChangeState(BossBehavior.EnemyState.Die);

                    yield break;
                }
                else
                {
                    onChangeState(BossBehavior.EnemyState.Damaged);
                }
                yield return new WaitForSeconds(0.5f);
            }

            if(currHP > 0)
            {
                // 계속 사망 상태 이후에 Idle로 잘못전환되는걸 방지함
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
            audioSource.PlayOneShot(damageSound, 0.5f);
            //Debug.Log($"{type} 타입의 데미지 발생");
        }
    }
}
