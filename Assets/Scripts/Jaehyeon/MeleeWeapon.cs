using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MeleeWeapone : MonoBehaviour
{
    public float attackRange = 2f; // 공격 범위
    public int damage = 25; // 공격 데미지
    public Transform attackPoint; // 공격 시작 지점

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // 마우스 우클릭을 감지하여 공격 실행
        if (Input.GetButtonDown("Fire1")) // 마우스 좌클릭
        {
            Attack();
        }
       
    }

    void Attack()
    {
        // 공격 범위 내의 충돌 감지
        RaycastHit hit;
        if (Physics.Raycast(attackPoint.position, attackPoint.forward, out hit, attackRange))
        {
            Debug.Log("Hit " + hit.transform.name);

            // 적에게 데미지를 줌
            EnemyHealth enemy = hit.transform.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }
}
