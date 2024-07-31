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
    public bool needMag = true; // 탄창이 필요한지 여부

    public int weaponeIndex = 0;

    private float lastFireTime = 0f;

    // 마나 아이템 사용시 소모
    public int manaCost = 1; // 마법 사용 시 소모되는 마나

    public bool isMagic = false;

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (isMagic)
            {
                UseManaItem();
            }
            else
            {
                Reload();
            }
            return;
        }

        if (isMagic)
        {
            HandleMagicFire();
        }
        else
        {
            HandleWeaponFire();
        }
    }

    void HandleWeaponFire()
    {
        if (AutoFire)
        {
            if (Input.GetButton("Fire1") && Time.time - lastFireTime >= fireRate)
            {
                if (!needMag || UiManager.instance.BulletShoot())
                {
                    Shoot();
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
                if (!needMag || UiManager.instance.BulletShoot())
                {
                    Shoot();
                }
                else
                {
                    Debug.Log("탄약 부족. 재장전 필요.");
                }
            }
        }
    }

    void HandleMagicFire()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (TryUseMana())
            {
                UiManager.instance.ManaRefresh(manaCost);
                Shoot();
            }
            else
            {
                UiManager.instance.UseMana(); // 마나 아이템 자동 사용
            }
        }
    }

    bool TryUseMana()
    {
        if (UiManager.instance.currentMana < 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    void UseManaItem()
    {
        UiManager.instance.UseMana();
    }

    public void Shoot()
    {
        if (effectPrefab == null)
        {
            Debug.LogError("Inspector창에 무기상태가 안들어감");
            return;
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
                BossDamaged bossDamaged = hitInfo.collider.GetComponent<BossDamaged>();
                bossDamaged.Damaged(damage, type);
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

            UiManager.instance.Reload(weaponeIndex);
        }
    }
}
