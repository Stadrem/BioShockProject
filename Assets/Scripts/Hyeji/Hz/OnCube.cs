using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnCube : MonoBehaviour
{
    // 보스행동 스크립트
    private BossBehavior bossBehavior;

    void Start()
    {
        // bossBehavior 스크립트 참조
        bossBehavior = GetComponentInParent<BossBehavior>();
    }


    void Dash()
    {
        bossBehavior.StartCoroutine(bossBehavior.ActivateDamageTrigger());
    }
}
