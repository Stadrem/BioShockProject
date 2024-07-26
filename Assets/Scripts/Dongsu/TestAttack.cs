using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAttack : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {

            print("발싸");
            //카메라 위치에서 카메라 앞 방향으로 향하는 Ray를 만들어서
            Ray ray = new Ray(transform.position, transform.forward);

            //Ray를 발사해서 어딘가 부딪힘
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo))
            {
                print("부딪");
                if (hitInfo.collider.gameObject.CompareTag("Enemy"))
                {
                    print("에너미 인식");
                    Damaged damaged = hitInfo.collider.GetComponent<Damaged>();
                    damaged.Damage(1, "데미지타입");
                }
                else if (hitInfo.collider.gameObject.CompareTag("Boss"))
                {
                    //BossDamaged bossDamaged = hitInfo.collider.GetComponents<BossDamaged>();
                    //bossDamaged.BossDamage(1);
                }
                else
                {
                    print("인식 실패");
                }
            }
        }
    }
}
