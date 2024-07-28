using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotalWeapon : MonoBehaviour
{
    public LayerMask layerMask;
    public GameObject effectPrefab; // 무기 또는 마법 효과 프리팹
    public float attackRange = 100f; // 공격 범위
    public int damage = 5; // 공격 데미지
    public string type = "?";

    // 총기류 관련 변수
    public bool AutoFire = false; // 자동 발사 여부 (기관총 등)
    public float fireRate = 0.1f; // 발사 간격 (자동 발사용)
    public int magazineSize = 30; // 탄창 크기
    public int currentBullet; // 현재 탄약 수
    public bool needMag = true; // 탄창이 필요한지 여부

    private float lastFireTime = 0f;

    void Start()
    {
        currentBullet = magazineSize;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
            return;
        }

        // 만약 자동 발사 모드가 on 이라면
        if (AutoFire)
        {
            // 마우스 좌클릭이 눌린 상태, 이전 발사 이후로 fireRate만큼의 시간이 지skaus
            if (Input.GetButton("Fire1") && Time.time - lastFireTime >= fireRate)
            {
                // 탄창이 필요하지 않거나, 현재 탄약이 0보다 크다면 발사
                if (!needMag || currentBullet > 0)
                {
                    Shoot();
                    // 마지막 발사 시간을 현재 시간으로 갱신
                    lastFireTime = Time.time;
                }
                else
                {
                    Debug.Log("탄약 부족. 재장전 필요.");
                }
            }
        }
        else
        {
            if (Input.GetButtonDown("Fire1"))
            {
                
                if (!needMag || currentBullet > 0)
                {
                    Debug.Log(2);
                    Shoot();
                }
                else
                {
                    Debug.Log("탄약 부족. 재장전 필요.");
                }
            }
        }
    }

    public void Shoot()
    {
        if (effectPrefab == null)
        {
            Debug.LogError("Inspector창에 무기상태가 안들어감");
            return;
        }

        // 총기류의 경우 탄약 감소
        if (needMag)
        {
            currentBullet--;
        }

        // Raycast를 이용해 적 감지 및 데미지 적용
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, attackRange, layerMask))
        {
            Debug.Log(hitInfo.transform.name);
            if (hitInfo.collider.CompareTag("Enemy"))
            {
                Damaged damaged = hitInfo.collider.GetComponent<Damaged>();
                damaged.Damage(damage, type);
            }
            else if (hitInfo.collider.CompareTag("Boss"))
            {
                //BossDamaged bossDamaged = hitInfo.collider.GetComponent<BossDamaged>();
                //bossDamaged.BossDamage(1);
            }

            // 파편 효과 생성
            GameObject bulletImpact = Instantiate(effectPrefab);
            bulletImpact.transform.position = hitInfo.point;
            bulletImpact.transform.forward = hitInfo.normal;
            Destroy(bulletImpact, 2); // 2초 뒤에 파괴
        }
    }

    void Reload()
    {
        if (needMag)
        {
            Debug.Log("장전중...");
            currentBullet = magazineSize;
            Debug.Log("장전완료. 현재 총알: " + currentBullet);
        }
    }
}
