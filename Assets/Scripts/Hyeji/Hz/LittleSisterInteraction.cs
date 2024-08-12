using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LittleSisterInteraction : MonoBehaviour
{

    // 플레이어 Transform
    Transform player;
    // 상호작용 UI
    public GameObject interactionUI;
    // 상호작용 거리
    public float interactionDistance = 5f;
    // 빅대디 죽었니?
    private bool isDead = false;

    BossBehavior bossBehavior;
    private Animator anim;
    private NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").transform;
        // 처음에는 UI 꺼두기
        interactionUI.SetActive(false);

        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        bossBehavior = GetComponent<BossBehavior>();
    }

    // Update is called once per frame
    void Update()
    {

        //if(BossBehavior.ChangeState(BossBehavior.EnemyState.Die)
        //{
        //    bossBehavior.ChangeState(BossBehavior.EnemyState.Die
        //}


        //if (isDead)
        //{
        //    // 리틀 시스터랑 플레이어 거리 구하기
        //    float dist = Vector3.Distance(transform.position, player.position);
        //    // 상호작용 거리에 들어오면
        //    if (dist <= interactionDistance)
        //    {
        //        // 활성화 시키자
        //        interactionUI.SetActive(true);

        //        if (Input.GetKeyDown(KeyCode.L))
        //        {
        //            // 호출 함수
        //            Interact();
        //        }
        //        else
        //        {
        //            interactionUI.SetActive(false);
        //        }
        //    }
        //}
    }

    // 빅대디가 죽었을 때 호출하는 메서드
    public void OnBigDaddyDeath()
    {
        isDead = true;
    }



    void Interact()
    {
        print("상호작용 완료");
        // 사라지게 해라
        Destroy(this.gameObject);
    }
}
