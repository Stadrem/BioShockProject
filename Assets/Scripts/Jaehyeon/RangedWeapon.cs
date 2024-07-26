using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedWeapon : WeaponBase
{
    public WeaponState weaponState; // 무기 상태

    public override void Use()
    {
        Shoot();
    }

    void Shoot()
    {
        if (weaponState == null)
        {
            Debug.LogError("WeaponState is not assigned in the inspector.");
            return;
        }

        // Raycast를 이용해 적 감지 및 데미지 적용
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, weaponState.attackRange))
        {
            if (hitInfo.collider.CompareTag("Enemy"))
            {
                Damaged damaged = hitInfo.collider.GetComponent<Damaged>();
                damaged.Damage(weaponState.damage, "Shot");
            }
            else if (hitInfo.collider.CompareTag("Boss"))
            {
                //BossDamaged bossDamaged = hitInfo.collider.GetComponent<BossDamaged>();
                //bossDamaged.BossDamage(1);
            }

            // 총알 파편 효과 생성
            GameObject bulletImpact = Instantiate(weaponState.prefab);
            bulletImpact.transform.position = hitInfo.point;
            bulletImpact.transform.forward = hitInfo.normal;
            Destroy(bulletImpact, 2); // 2초 뒤에 파괴
        }
    }
}
