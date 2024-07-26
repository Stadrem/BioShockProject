using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDamaged : MonoBehaviour
{
    public enum DamageType
    {
        // 일시정지, 도트 피해, 데미지 없는 긴 일시 정지, 근접, 원거리
        Shock,
        Fire,
        Ice,
        Melee,
        Shot
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Damaged(int damage, DamageType type)
    {
        switch (type)
        {
            case DamageType.Shock:
                StunDamageStep(damage, 1.0f);
                break;
            case DamageType.Fire:
                DamageStep(damage, 5);
                break;
            case DamageType.Ice:
                StunDamageStep(damage, 3.0f);
                break;
            default:
                DamageStep(damage, 1);
                break;

        }
    }

    void StunDamageStep(int damage, float stunDuration)
    {

    }

    void DamageStep(int damage, float multiplier)
    {

    }
}
