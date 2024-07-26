using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//���콺�� �����ӿ� ����
//ī�޶�, ĳ���͸� ȸ���ϰ� �ʹ�.



public class ObjRotate : MonoBehaviour
{
    //ȸ���� 2(���콺�� �������� �����ϴ� ��)
    float rotX = 0;
    float rotY = 0;

    //ȸ�� ���ǵ� == �������� ���� �ʹ� ����, �������� ��ȭ ���� ���⿡ �ش� �ڵ� ���
    float rotSpeed = 200;

    //ȸ�� ���� ���� ==�ϳ��� ī�޶� ����ְ� �ϳ��� �÷��̾ ������. RotX�� �÷��̾, RotY�� ����ī�޶� 
    public bool useRotX;
    public bool useRotY;

    //region ���۷��� Ÿ�԰� value Ÿ���� ��� ��ȯ�Ǿ��� �� ������� �������/ ����� 
    //void TestFunc()
    //{
    // MyTransform myTransform = new Mytransform();
    // }
    /*void Start()
    {
        
    }*/

    // Update is called once per frame
    void Update()
    {
        //1. ���콺�� �����Ӱ��� �޾ƿ���(�¿����)
        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");
        //2. ���콺�� �����Ӱ��� ������Ű��

        if (useRotY)
        {
            rotY += mx  * rotSpeed * Time.deltaTime;
        }


        if (useRotX)
        {
            rotX += my  * rotSpeed * Time.deltaTime;
        }


        //rotY += my * Time.deltaTime * rotSpeed;

        //rotX�� ���� -80 80���� ����
        rotX = Mathf.Clamp(rotX, -80, 80);

        //3. ������ ���� ��ü�� ȸ�������� ��������. localEulerAngles(0~360�� ����)
        transform.localEulerAngles = new Vector3(-rotX, rotY, 0);
    }
}

