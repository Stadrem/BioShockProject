using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//마우스의 움직임에 따라
//카메라, 캐릭터를 회전하고 싶다.



public class ObjRotate : MonoBehaviour
{
    //회전값 2(마우스의 움직임을 누적하는 값)
    float rotX = 0;
    float rotY = 0;

    //회전 스피드 == 도리도리 각이 너무 작음, 움직임의 변화 거의 없기에 해당 코드 사용
    float rotSpeed = 200;

    //회전 가능 여부 ==하나는 카메라에 집어넣고 하나는 플레이어에 넣을것. RotX는 플레이어에, RotY는 메인카메라에 
    public bool useRotX;
    public bool useRotY;

    //region 레퍼런스 타입과 value 타입의 경우 반환되었을 때 멤버변수 접근허용/ 비허용 
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
        //1. 마우스의 움직임값을 받아오자(좌우상하)
        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");
        //2. 마우스의 움직임값을 누적시키자

        if (useRotY)
        {
            rotY += mx  * rotSpeed * Time.deltaTime;
        }


        if (useRotX)
        {
            rotX += my  * rotSpeed * Time.deltaTime;
        }


        //rotY += my * Time.deltaTime * rotSpeed;

        //rotX의 값을 -80 80도로 제한
        rotX = Mathf.Clamp(rotX, -80, 80);

        //3. 누적된 값을 물체의 회전값으로 셋팅하자. localEulerAngles(0~360값 세팅)
        transform.localEulerAngles = new Vector3(-rotX, rotY, 0);
    }
}

