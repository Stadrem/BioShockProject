using System.Collections;
using System.Collections.Generic;
using System.Threading;
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

    //private bool Knockback = false;
    private Vector3 backDirection;
    //private float knockbackTime;
    //private float knockbackDuration = 0.5f;

    public float Walktime = 1;
    public float currenttime;
    public AudioClip MoveSound;
    public AudioClip JumpSound;
    private AudioSource audioSource;

    void Start()
    {
        // 캐릭터 컨트롤러 가져오기
        cc = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
        //Cursor.lockState = CursorLockMode.Confined;
    }

    void Update()
    {
        // 사용자의 입력을 받아서 방향을 만듦
        float h = Input.GetAxis("Horizontal"); // a = -1, d = 1, 눌르지 않으면 0
        float v = Input.GetAxis("Vertical"); // s = -1, w = 1, 눌르지 않으면 0

        Vector3 dirH = transform.right * h;
        Vector3 dirV = transform.forward * v;
        Vector3 dir = dirH + dirV;

        if(dir.sqrMagnitude > 0)
        {
            // 시간흐르게
            currenttime += Time.deltaTime;
            // 0.1 보다 크면
            if(currenttime > Walktime)
            {
                // 소리한번 내고
                if (MoveSound != null && audioSource != null)
                {
                    audioSource.PlayOneShot(MoveSound);
                }
                // 초기화
                currenttime = 0;
            }

            
        }
        
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
            if (JumpSound != null && audioSource != null)
            {
                //audioSource.Play();
                //audioSource.time = 0.5f;
                audioSource.PlayOneShot(JumpSound);
            }
        }

        // yVelocity 값을 점점 줄여줌 (중력에 의해서)
        yVelocity += gravity * Time.deltaTime;

        // dir의 y값에 yVelocity를 세팅함
        dir.y = yVelocity;

      

        // 이동
        cc.Move(dir * speed * Time.deltaTime);
    }

    /*public void ApplyKnockback(Vector3 direction)
    {

        direction.y = 0;

        backDirection = direction.normalized;
        knockbackTime = Time.time;
        Knockback = true;

    }
     56 6 ,olklm /*private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Boss"))
       {
            Collider[] colls;
            colls = Physics.OverlapSphere(this.transform.position, 1f, 1 << LayerMask.NameToLayer("Boss"));

            foreach (Collider coll in colls)
            {

                coll.GetComponent<Rigidbody>().AddExplosionForce(300, transform.position, 10f);
            }

            Destroy(this.gameObject);
        }
        
    }*/
}
