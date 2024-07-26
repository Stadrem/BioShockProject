using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotalWeapon : MonoBehaviour
{
    //public GameObject prefab; // 무기 상태
    public LayerMask layerMask;
    public GameObject effectPrefab; // 무기 또는 마법 효과프리팹
    public float attackRange = 100f; // 공격 범위
    public int damage = 5; // 공격 데미지
    public string type = "?";

    public void Shoot()
    {
        if (effectPrefab == null)
        {
            Debug.LogError("WeaponState is not assigned in the inspector.");
            return;
        }

        // Raycast를 이용해 적 감지 및 데미지 적용
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, attackRange, layerMask))
        {
            print(hitInfo.transform.name);
            if (hitInfo.collider.CompareTag("Enemy"))
            {
                print("????");
                Damaged damaged = hitInfo.collider.GetComponent<Damaged>();
                damaged.Damage(damage, type);
            }
            else if (hitInfo.collider.CompareTag("Boss"))
            {
                //BossDamaged bossDamaged = hitInfo.collider.GetComponent<BossDamaged>();
                //bossDamaged.BossDamage(1);
            }

            //파편 효과 생성
            GameObject bulletImpact = Instantiate(effectPrefab);
            bulletImpact.transform.position = hitInfo.point;
            bulletImpact.transform.forward = hitInfo.normal;
            Destroy(bulletImpact, 2); // 2초 뒤에 파괴
        }
    }
}
