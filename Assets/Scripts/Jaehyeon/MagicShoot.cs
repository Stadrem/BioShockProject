using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicShoot : MonoBehaviour
{
    public WeaponState weaponState; // 마법 상태

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && gameObject.activeSelf) // 마우스 좌클릭
        {
            ShootMagic();
        }
    }

    public void ShootMagic()
    {
        if (weaponState == null)
        {
            Debug.LogError("WeaponState is not assigned in the inspector.");
            return;
        }

        // Raycast를 이용해 적 감지 및 데미지 적용
        Ray ray = new Ray(weaponState.firePoint.position, weaponState.firePoint.forward);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, weaponState.attackRange))
        {
            if (hitInfo.collider.CompareTag("Enemy"))
            {
                Damaged damaged = hitInfo.collider.GetComponent<Damaged>();
                damaged.Damage(weaponState.damage, "Magic");
            }
            else if (hitInfo.collider.CompareTag("Boss"))
            {
                //BossDamaged bossDamaged = hitInfo.collider.GetComponent<BossDamaged>();
                //bossDamaged.BossDamage(1);
            }

            // 마법 충돌 효과 생성
            GameObject magicImpact = Instantiate(weaponState.prefab, hitInfo.point, Quaternion.identity);
            Destroy(magicImpact, 2); // 2초 뒤에 파괴
        }
    }
}
