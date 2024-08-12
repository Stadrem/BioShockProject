using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnGroundSlam : MonoBehaviour
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
}
