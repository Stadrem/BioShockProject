using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MeleeWeapone : MonoBehaviour
{
    public float attackRange = 2f; // ���� ����
    public int damage = 25; // ���� ������
    public Transform attackPoint; // ���� ���� ����

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // ���콺 ��Ŭ���� �����Ͽ� ���� ����
        if (Input.GetButtonDown("Fire1")) // ���콺 ��Ŭ��
        {
            Attack();
        }
       
    }

    void Attack()
    {
        // ���� ���� ���� �浹 ����
        RaycastHit hit;
        if (Physics.Raycast(attackPoint.position, attackPoint.forward, out hit, attackRange))
        {
            Debug.Log("Hit " + hit.transform.name);

            // ������ �������� ��
            EnemyHealth enemy = hit.transform.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }
}
