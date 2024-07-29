using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : WeaponBase
{
    public WeaponState weaponState; // 무기 상태
    public LayerMask layerMask;

    public override void Use()
    {
        Attack();
    }

    void Attack()
    {
        if (weaponState == null)
        {
            Debug.LogError("WeaponState is not assigned in the inspector.");
            return;
        }

        // Raycast를 이용해 적 감지 및 데미지 적용
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, weaponState.attackRange, layerMask))
        {
            print(hitInfo.transform.name);
            
            if (hitInfo.collider.CompareTag("Enemy"))
            {
                Damaged damaged = hitInfo.collider.GetComponent<Damaged>();
                damaged.Damage(weaponState.damage, "Melee");
            }
            else if (hitInfo.collider.CompareTag("Boss"))
            {
                //BossDamaged bossDamaged = hitInfo.collider.GetComponent<BossDamaged>();
                //bossDamaged.BossDamage(1);
            }
        }
    }
}
