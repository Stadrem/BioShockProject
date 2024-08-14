using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Interaction : MonoBehaviour
{
    ParticleSystem ps;

    public Canvas lManager;
    public Image interactionUI;
    public Text interactionMessage;

    public GameObject destroyEffect;
    

    Transform littleSister;

    // 접근했는가?
    bool contact = false;


    private void Start()
    {
        littleSister = GameObject.Find("LittleSister").transform;
        lManager = GameObject.Find("LManger").GetComponent<Canvas>();

        interactionMessage.gameObject.SetActive(false);
        interactionUI.gameObject.SetActive(false);

        ps = GetComponent<ParticleSystem>();
    }

    // Start is called before the first frame update
    void Update()
    {
        // L누르고 접촉했을때
        if (Input.GetKeyDown(KeyCode.L) && contact == true)
        {
            Collider sistercollider = littleSister.GetComponent<Collider>();
            if (sistercollider != null)
            {
                Exit();
                sistercollider.enabled = false;
                //OnTriggerExit(sistercollider);
            }

            // 없어지면서 효과 나오게하기 (이펙트)
            Instantiate(destroyEffect, transform.position, transform.rotation);
            // 리틀 시스터 오브젝트 없애기
            Destroy(littleSister.gameObject);       
            //StartCoroutine(DestroyAfterDelay());
                   
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 플레이어가 다가왔을때 발동
        if(other.gameObject.CompareTag("Player"))
        {
            print("가까워졌는가?");

            contact = true;
            //lManager.gameObject.SetActive(true);

            interactionMessage.gameObject.SetActive(true);
            interactionUI.gameObject.SetActive(true);
        }

    }

    private void OnTriggerExit(Collider other)
    {
        // 탈출
        if(other.gameObject.CompareTag("Player"))
        {
            Exit();
        }
    }

    void Exit()
    {
        print("탈출했는가?");

        contact = false;
        //gameObject.SetActive(false);

        interactionMessage.gameObject.SetActive(false);
        interactionUI.gameObject.SetActive(false);
    }
}
