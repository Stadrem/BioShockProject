using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerFire : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject bulletImpactFactory;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //마우스 우클릭을 눌렀을 때
        if (Input.GetMouseButtonDown(1))
        {
            //카메라 위치에서 카메라 앞방향으로 향하는 Ray를 만들자. 
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            //Ray를 발사해서 어딘가에 부딪혔다면(Physics라는 걸로 발사가능)
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo))
            {
                //총알 파편 효과를 생성하자.
                GameObject bulletImpact = Instantiate(bulletImpactFactory);
                //생성된 효과를 부딪힌 위치에 두자
                bulletImpact.transform.position = hitInfo.point;
                //생성된 효과의 앞방향을 부딪힌 위치의 normal방향으로 설정
                bulletImpact.transform.forward = hitInfo.normal;

                //Vector3 outDirection = Vector3.Reflect(ray.direction, hitInfo.normal);
               // bulletImpact.transfrom.foward = outDirection;


                //부딪힌 오브젝트의 이름과 부딪힌 위치를 출력해보자.
                print(hitInfo.transform.name + "," + hitInfo.point);
                print(hitInfo.transform.name + "," + hitInfo.transform.position);
                //발사한 위치와 거기까지의 거리가 들어가있다. 포인터를 알면 발사위치에서 포인트러르 뺸 렝스의 벡터의 길이를 구하거나, 
                
                /*Vector3.Distance(Camera.main.transform.position, hitInfo.point);
                Vector3 dist = Camera.main.transform.position, hitInfo.point;
                dist.magnitude; //벡터의 길이
                hitInfo.distance;*/

                //법선벡터 : 면의 수직인 벡터를 의미 == hitInfo.normal
                // Ray의 시작 위치에서 부딪힌 위치까지의 거리 == hitInfo.distance
            }


        }


    }
}
