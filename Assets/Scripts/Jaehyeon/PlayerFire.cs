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
        //���콺 ��Ŭ���� ������ ��
        if (Input.GetMouseButtonDown(1))
        {
            //ī�޶� ��ġ���� ī�޶� �չ������� ���ϴ� Ray�� ������. 
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            //Ray�� �߻��ؼ� ��򰡿� �ε����ٸ�(Physics��� �ɷ� �߻簡��)
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo))
            {
                //�Ѿ� ���� ȿ���� ��������.
                GameObject bulletImpact = Instantiate(bulletImpactFactory);
                //������ ȿ���� �ε��� ��ġ�� ����
                bulletImpact.transform.position = hitInfo.point;
                //������ ȿ���� �չ����� �ε��� ��ġ�� normal�������� ����
                bulletImpact.transform.forward = hitInfo.normal;

                //Vector3 outDirection = Vector3.Reflect(ray.direction, hitInfo.normal);
               // bulletImpact.transfrom.foward = outDirection;


                //�ε��� ������Ʈ�� �̸��� �ε��� ��ġ�� ����غ���.
                print(hitInfo.transform.name + "," + hitInfo.point);
                print(hitInfo.transform.name + "," + hitInfo.transform.position);
                //�߻��� ��ġ�� �ű������ �Ÿ��� ���ִ�. �����͸� �˸� �߻���ġ���� ����Ʈ���� �A ������ ������ ���̸� ���ϰų�, 
                
                /*Vector3.Distance(Camera.main.transform.position, hitInfo.point);
                Vector3 dist = Camera.main.transform.position, hitInfo.point;
                dist.magnitude; //������ ����
                hitInfo.distance;*/

                //�������� : ���� ������ ���͸� �ǹ� == hitInfo.normal
                // Ray�� ���� ��ġ���� �ε��� ��ġ������ �Ÿ� == hitInfo.distance
            }


        }


    }
}
