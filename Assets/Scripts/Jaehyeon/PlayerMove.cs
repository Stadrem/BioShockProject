using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{

    //움직이는 스피드
    public float speed = 5;

    //캐릭터 컨트롤러
    CharacterController cc;

    //아래 3가지(중력, 수직속력, 점프파워) 이렇게 세가지가 전역변수로 설정되어 있어야, 점프동작 기초 구현
    //중력
    public float gravity = -20;

    //수직속력(y방향 속력) 
    public float yVelocity = 0;

    //점프파워
    public float jumpPower = 4;

    //최대 점프 횟수
    public int jumpCount = 1;

    //현재 점프 횟수
    int jumpCurrCnt;

    // 앉기 상태와 속도
    private bool isCrouching = false;
    public float crouchSpeed = 2.5f;
    public float crouchHeight = 1.0f;
    public float standHeight = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        //캐릭터 컨트롤러 가져오기
        cc = GetComponent<CharacterController>();

    }

    // Update is called once per frame
    void Update()
    {
        //1. 사용자의 입력을 받아야함(w, a, s, d)
        float h = Input.GetAxis("Horizontal"); //a = -1, d = 1 눌르지 않으면 0
        float v = Input.GetAxis("Vertical"); //s = -1 w=1 눌르지 않으면 0

        //2. 값에 따라서 방향을 만는다. ex(1, 0, 0) * h == a키를 눌렀을때 d키를 누르게 되면 (-1, 0, 0)이 되기에 왼쪽
        Vector3 dirH = transform.right * h;
        Vector3 dirV = transform.forward * v;
        Vector3 dir = dirH + dirV;

        dir.Normalize();

        // 앉기 처리
        if (Input.GetKeyDown(KeyCode.C)) //C키를 눌렀을때 앉기
        {
            isCrouching = !isCrouching;
            if (isCrouching)
            {
                cc.height = crouchHeight;
                speed = crouchSpeed;
            }
            else
            {
                cc.height = standHeight;
                speed = 5;
            }
        }


        //만약에 땅에 있다면 yVelocity를 0으로 초기화하자 //하지만 이럴경우 점프가 될때랑 안될때가 발생하게 된다. 이를 해결하기 위해 아래 코드를 사용한다. 
        if (cc.isGrounded)
        {
            yVelocity = 0;
            jumpCurrCnt = 0;
        }


        // 4. 만약에 스페이스 바를 누르면

        if (Input.GetButtonDown("Jump")) // #2 if(input.GetKeyDown(Keycode.space));
        {

            //만약에 점프할 수 있다면, 점프를 하자 점프횟수를 하나 증가시키자.
            if (jumpCurrCnt < jumpCount)
            {
                //yVelocity를 jumpPower 한다.
                yVelocity = jumpPower;

                //점프 하나 증가
                jumpCurrCnt++;


            }
        }

        //yVelocity 값을 점점 줄여준다. (중력에 의해서) //이렇게 된 경우 지면에 올라가있다가 바닥에 떨어질때 확 떨어지게 된다. 
        yVelocity += gravity * Time.deltaTime;

        //dir의 y값에 yVelocity를 세팅한다. 
        dir.y = yVelocity;


        //3. 그 방향으로 계속 이동하고 싶다. 
        //이동 공식 p = p0 + vt
        //transform.position = transform.position + dir * 5 * Time.deltaTime;
        cc.Move(dir * speed * Time.deltaTime);

    }
}
