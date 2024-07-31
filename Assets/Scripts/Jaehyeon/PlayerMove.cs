using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float speed = 5; // 움직이는 스피드
    public float gravity = -20; // 중력
    public float jumpPower = 4; // 점프 파워
    public float crouchSpeed = 2.5f; // 앉기 속도
    public float crouchHeight = 1.0f; // 앉았을 때 높이
    public float standHeight = 2.0f; // 서있을 때 높이

    private CharacterController cc; // 캐릭터 컨트롤러
    private float yVelocity = 0; // 수직 속력
    private bool isCrouching = false; // 앉기 상태
    private bool canJump = true; // 점프 가능 상태

    void Start()
    {
        // 캐릭터 컨트롤러 가져오기
        cc = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Confined;
    }

    void Update()
    {
        // 사용자의 입력을 받아서 방향을 만듦
        float h = Input.GetAxis("Horizontal"); // a = -1, d = 1, 눌르지 않으면 0
        float v = Input.GetAxis("Vertical"); // s = -1, w = 1, 눌르지 않으면 0

        Vector3 dirH = transform.right * h;
        Vector3 dirV = transform.forward * v;
        Vector3 dir = dirH + dirV;

        dir.Normalize();

        // 앉기 처리
        if (Input.GetKeyDown(KeyCode.C))
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

        // 땅에 있는 경우 yVelocity 초기화하고 점프 가능하게 설정
        if (cc.isGrounded)
        {
            yVelocity = 0;
            canJump = true; // 점프 가능 상태로 전환
        }

        // 스페이스 바를 누르면 점프
        if (Input.GetButtonDown("Jump") && canJump) // 점프 가능 상태 확인
        {
            yVelocity = jumpPower;
            canJump = false; // 점프 후에는 점프 불가능 상태로 전환
        }

        // yVelocity 값을 점점 줄여줌 (중력에 의해서)
        yVelocity += gravity * Time.deltaTime;

        // dir의 y값에 yVelocity를 세팅함
        dir.y = yVelocity;

        // 이동
        cc.Move(dir * speed * Time.deltaTime);
    }
}
