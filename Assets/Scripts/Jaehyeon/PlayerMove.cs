using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    // 움직이는 스피드
    public float speed = 5;

    // 캐릭터 컨트롤러
    CharacterController cc;

    // 중력
    public float gravity = -9.81f; // 중력을 약하게 설정
    // 수직속력(y방향 속력) 
    public float yVelocity = 0;
    // 점프파워
    public float jumpPower = 2; // 점프 파워를 낮게 설정
    // 점프 딜레이
    public float jumpDelay = 0.2f; // 점프 딜레이 시간

    // 앉기 상태와 속도
    private bool isCrouching = false;
    public float crouchSpeed = 2.5f;
    public float crouchHeight = 1.0f;
    public float standHeight = 2.0f;

    // 점프 가능 여부
    private bool canJump = true;

    // Start is called before the first frame update
    void Start()
    {
        // 캐릭터 컨트롤러 가져오기
        cc = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        // 1. 사용자의 입력을 받아야함(w, a, s, d)
        float h = Input.GetAxis("Horizontal"); // a = -1, d = 1 눌르지 않으면 0
        float v = Input.GetAxis("Vertical"); // s = -1 w = 1 눌르지 않으면 0

        // 2. 값에 따라서 방향을 만듦
        Vector3 dirH = transform.right * h;
        Vector3 dirV = transform.forward * v;
        Vector3 dir = dirH + dirV;

        dir.Normalize();

        // 앉기 처리
        if (Input.GetKeyDown(KeyCode.C)) // C키를 눌렀을때 앉기
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

        // 만약에 땅에 있다면 yVelocity를 0으로 초기화
        if (cc.isGrounded)
        {
            yVelocity = 0;
        }

        // 4. 만약에 스페이스 바를 누르면
        if (Input.GetButtonDown("Jump") && cc.isGrounded && canJump)
        {
            StartCoroutine(JumpWithDelay());
        }

        // yVelocity 값을 점점 줄여줌 (중력에 의해서)
        yVelocity += gravity * Time.deltaTime;

        // dir의 y값에 yVelocity를 세팅
        dir.y = yVelocity;

        // 3. 그 방향으로 계속 이동하고 싶음
        cc.Move(dir * speed * Time.deltaTime);
    }

    // 점프에 딜레이를 주는 코루틴
    IEnumerator JumpWithDelay()
    {
        canJump = false;
        yield return new WaitForSeconds(jumpDelay);
        yVelocity = jumpPower;
        canJump = true;
    }
}
