using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnFirePos : MonoBehaviour
{
    // 보스행동 스크립트
    private rosieBehavior rosie;
    // 오디오 소스
    public AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        rosie = GetComponentInParent<rosieBehavior>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FirePos()
    {
        //rosie.Attack();
    }
}
