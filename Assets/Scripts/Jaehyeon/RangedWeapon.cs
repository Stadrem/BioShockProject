using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedWeapon : MonoBehaviour
{
    public GameObject bulletPrefab; // �Ѿ� ������
    public Transform firePoint; // �Ѿ� �߻� ��ġ
    public float bulletSpeed = 10f; // �Ѿ� �ӵ�
    public float attackRange = 100f; // ���� ����
    public int damage = 25; // ���� ������

    private void Start()
    {
        
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(1)) // ���콺 ��Ŭ��
        {
            Shoot();
        }
    }

    void Shoot()
    {
        // �Ѿ� �������� �����ϰ� �߻�
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.velocity = firePoint.forward * bulletSpeed;

        // Raycast�� �̿��� �� ���� �� ������ ����
        Ray ray = new Ray(firePoint.position, firePoint.forward);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, attackRange))
        {
            Debug.Log("Hit " + hitInfo.transform.name);

            // ������ ������ �ֱ�
            EnemyHealth enemy = hitInfo.transform.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            // �Ѿ� ���� ȿ�� ����
            GameObject bulletImpact = Instantiate(bulletPrefab);
            bulletImpact.transform.position = hitInfo.point;
            bulletImpact.transform.forward = hitInfo.normal;
            Destroy(bulletImpact, 2); // 2�� �ڿ� �ı�
        }
    }
}

