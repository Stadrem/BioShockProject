using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;

public class LittleSisterInteraction : MonoBehaviour
{
    // 빅대디의 Transform
    Transform bigDaddy;
    // 플레이어 Transform
    Transform player;
    // 리틀 시스터 Transform
    Transform littleSister;

    // 상호작용 UI
    public Image interactionUI;
    // Text
    public TextMeshProUGUI interactionMessage;
    // 상호작용 거리
    public float interactionDistance = 5f;
    // 빅대디 죽었니?
    private bool isBigDaddyDead = false;

    // 빅대디1 행동 스크립트
    BossBehavior bossBehavior;
    private Animator anim;
    private NavMeshAgent agent;

    // 오디오 소스
    public AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").transform;
        bigDaddy = GameObject.Find("BigDaddy").transform;
        littleSister = GameObject.Find("LittleSister").transform;

        // 널레퍼런스 방지하기
        if(player == null || bigDaddy == null || littleSister == null)
        {
            return;
        }

        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        bossBehavior = GetComponent<BossBehavior>();

        // 오디오 소스 가져오기
        audioSource = GetComponent<AudioSource>();

        // 처음에는 UI 꺼야함
        interactionUI.gameObject.SetActive(false);
        interactionMessage.gameObject.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        // 빅대디가 죽음 상태라면
        if(bossBehavior.state == BossBehavior.EnemyState.Die)
        {
            isBigDaddyDead = true;
        }

        // 상호작용 호출
        Interaction();
    }

    void Interaction()
    {
        // 빅대디가 죽으면
        if(isBigDaddyDead)
        {
            // 보스가 죽었고, 플레이어가 리틀 시스터와 상호작용 범위 내인가?
            float dir = Vector3.Distance(transform.position, littleSister.transform.position);
            if (dir <= interactionDistance)
            {
                // 상호작용 메시지랑 이미지 띄우기
                interactionMessage.gameObject.SetActive(true);
                interactionUI.gameObject.SetActive(true);

                // L 키를 누르면 구원 
                if (Input.GetKeyDown(KeyCode.L))
                {
                    // 구원뜨게 하는 로직 넣기
                    // 아 코루틴넣기
                    StartCoroutine(ShowScreen());
                }
            }
            // 만약 보스가 죽지 않고, 플레이어와 리틀 시스터가 상호작용 범위안 아니면
            else
            {
                // 일단 메세지 숨기기
                interactionMessage.gameObject.SetActive(false);
                interactionUI.gameObject.SetActive(false);
            }
        }      
    }

    // 코루틴으로 화면 표시
    IEnumerator ShowScreen()
    {
        // 메세지 숨김
        interactionMessage.gameObject.SetActive(false);
        // 화면 활성화(버튼 구원?)
        interactionUI.gameObject.SetActive(true);
        // 화면 2초 유지
        yield return new WaitForSeconds(2f);
        // 화면 다시 비활성화
        interactionUI.gameObject.SetActive(false);
        // 리틀 시스터 제거
        littleSister.gameObject.SetActive(false);

        if (audioSource != null)
        {
            audioSource.Play();
        }
    }

}
