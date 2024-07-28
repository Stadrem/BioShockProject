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

    // 총기류 관련 변수
    public bool isAutomatic = false; // 자동 발사 여부 (기관총 등)
    public float fireRate = 0.1f; // 발사 간격 (자동 발사용)
    public int magazineSize = 30; // 탄창 크기
    public int currentAmmo; // 현재 탄약 수
    public float reloadTime = 2f; // 재장전 시간

    private float lastFireTime = 0f;
    private bool isReloading = false;

    void Start()
    {
        currentAmmo = magazineSize;
    }
    void Update()
    {
        if (isAutomatic)
        {
            if (Input.GetButton("Fire1") && Time.time - lastFireTime >= fireRate && !isReloading)
            {
                if (currentAmmo > 0)
                {
                    Shoot();
                    lastFireTime = Time.time;
                }
                else
                {
                    StartCoroutine(Reload());
                }
            }
        }
        else
        {
            if (Input.GetButtonDown("Fire1") && !isReloading)
            {
                if (currentAmmo > 0)
                {
                    Shoot();
                }
                else
                {
                    StartCoroutine(Reload());
                }
            }
        }
    }
    public void Shoot()
    {
        if (effectPrefab == null)
        {
            Debug.LogError("WeaponState is not assigned in the inspector.");
            return;
        }

        // 총기류의 경우 탄약 감소
        if (type == "gun" || type == "rifle")
        {
            currentAmmo--;
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
    IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading...");

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = magazineSize;
        isReloading = false;
    }
}
