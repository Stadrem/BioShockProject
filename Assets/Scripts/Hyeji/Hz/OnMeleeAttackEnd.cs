using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnMeleeAttackEnd : MonoBehaviour
{
    // 보스행동 스크립트
    private BossBehavior bossBehavior;
    
    void Start()
    {
        // bossBehavior 스크립트 참조
        bossBehavior = GetComponentInParent<BossBehavior>();
    }

    public void MeleeAttackEnd()
    {
        // 플레이어와의 거리를 계산
        float dist = Vector3.Distance(bossBehavior.player.position, transform.position);

        // 거리에 따라 상태를 변경
        if (dist > bossBehavior.meleeAttackDistance && dist <= bossBehavior.shotAttackDistance)
        {
            bossBehavior.ChangeState(BossBehavior.EnemyState.ShotAttack);
        }
        else if (dist > bossBehavior.shotAttackDistance)
        {
            bossBehavior.ChangeState(BossBehavior.EnemyState.Move);
        }
    }
}
