using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedWeapon : MonoBehaviour
{
    public GameObject bulletPrefab; // 총알 프리팹
    public Transform firePoint; // 총알 발사 위치
    public float bulletSpeed = 10f; // 총알 속도
    public float attackRange = 100f; // 공격 범위
    public int damage = 25; // 공격 데미지

    private void Start()
    {
        
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(1)) // 마우스 우클릭
        {
            Shoot();
        }
    }

    void Shoot()
    {
        // 총알 프리팹을 생성하고 발사
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.velocity = firePoint.forward * bulletSpeed;

        // Raycast를 이용해 적 감지 및 데미지 적용
        Ray ray = new Ray(firePoint.position, firePoint.forward);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, attackRange))
        {
            Debug.Log("Hit " + hitInfo.transform.name);

            // 적에게 데미지 주기
            EnemyHealth enemy = hitInfo.transform.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            // 총알 파편 효과 생성
            GameObject bulletImpact = Instantiate(bulletPrefab);
            bulletImpact.transform.position = hitInfo.point;
            bulletImpact.transform.forward = hitInfo.normal;
            Destroy(bulletImpact, 2); // 2초 뒤에 파괴
        }
    }
}

