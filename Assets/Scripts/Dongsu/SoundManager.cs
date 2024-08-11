using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    AudioSource audioSource;

    public AudioClip damagedAudio;

    public AudioClip rootAudio;

    public AudioClip selectSound;

    public AudioClip paySound;

    public AudioClip failSound;

    public AudioClip healSound;

    private void Awake()
    {
        //instance 값이 null이면
        if (instance == null)
        {
            //이 스크립트를 instance에 담음
            instance = this;

            //씬 전환해도 유지하는 코드
            DontDestroyOnLoad(gameObject);
        }
        //이미 instance에 무언가 값이 들어있다면?
        else
        {
            //의도치 않은 중복 적용일 태니 이 게임 오브젝트 파괴.
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        transform.position = GameManager.instance.player.transform.position;

        transform.SetParent(GameManager.instance.player.transform);
    }

    void PlaySound(AudioClip audios, float volume)
    {
        audioSource.PlayOneShot(audios, volume);
    }

    public void DamagedSound()
    {
        PlaySound(damagedAudio, 0.5f);
    }

    public void RootSound()
    {
        PlaySound(rootAudio, 0.5f);
    }

    public void SelectSound()
    {
        PlaySound(selectSound, 0.8f);
    }

    public void PaySound()
    {
        PlaySound(paySound, 0.8f);
    }

    public void FailSound()
    {
        PlaySound(failSound, 0.5f);
    }

    public void HealSound()
    {
        PlaySound(healSound, 0.5f);
    }
}
