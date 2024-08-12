using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTriggerCube : MonoBehaviour
{
    // 보스행동 스크립트
    private BossBehavior bossBehavior;

    // Start is called before the first frame update
    void Start()
    {
        // bossBehavior 스크립트 참조
        bossBehavior = GetComponentInParent<BossBehavior>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 공격시 충돌 처리 
    private void OnTriggerEnter(Collider other)
    {
        print(bossBehavior.state);

        // 맞은 대상이 플레이어라면
        if (other.CompareTag("Player"))
        {
            // 플레이어에게 피해를 입힌다.
            GameManager.instance.Damaged(bossBehavior.attackPower);
            Debug.Log("플레이어에게 피해 입힘");
        }
    }
}
