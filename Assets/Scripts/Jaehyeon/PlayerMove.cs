using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{

    //�����̴� ���ǵ�
    public float speed = 5;

    //ĳ���� ��Ʈ�ѷ�
    CharacterController cc;

    //�Ʒ� 3����(�߷�, �����ӷ�, �����Ŀ�) �̷��� �������� ���������� �����Ǿ� �־��, �������� ���� ����
    //�߷�
    public float gravity = -20;

    //�����ӷ�(y���� �ӷ�) 
    public float yVelocity = 0;

    //�����Ŀ�
    public float jumpPower = 5;

    //�ִ� ���� Ƚ��
    public int jumpCount = 2;

    //���� ���� Ƚ��
    int jumpCurrCnt;


    // Start is called before the first frame update
    void Start()
    {
        //ĳ���� ��Ʈ�ѷ� ��������
        cc = GetComponent<CharacterController>();

    }

    // Update is called once per frame
    void Update()
    {
        //1. ������� �Է��� �޾ƾ���(w, a, s, d)
        float h = Input.GetAxis("Horizontal"); //a = -1, d = 1 ������ ������ 0
        float v = Input.GetAxis("Vertical"); //s = -1 w=1 ������ ������ 0

        //2. ���� ���� ������ ���´�. ex(1, 0, 0) * h == aŰ�� �������� dŰ�� ������ �Ǹ� (-1, 0, 0)�� �Ǳ⿡ ����
        Vector3 dirH = transform.right * h;
        Vector3 dirV = transform.forward * v;
        Vector3 dir = dirH + dirV;

        dir.Normalize();


        //���࿡ ���� �ִٸ� yVelocity�� 0���� �ʱ�ȭ���� //������ �̷���� ������ �ɶ��� �ȵɶ��� �߻��ϰ� �ȴ�. �̸� �ذ��ϱ� ���� �Ʒ� �ڵ带 ����Ѵ�. 
        if (cc.isGrounded)
        {
            yVelocity = 0;
            jumpCurrCnt = 0;
        }


        // 4. ���࿡ �����̽� �ٸ� ������

        if (Input.GetButtonDown("Jump")) // #2 if(input.GetKeyDown(Keycode.space));
        {

            //���࿡ ������ �� �ִٸ�, ������ ���� ����Ƚ���� �ϳ� ������Ű��.
            if (jumpCurrCnt < jumpCount)
            {
                //yVelocity�� jumpPower �Ѵ�.
                yVelocity = jumpPower;

                //���� �ϳ� ����
                jumpCurrCnt++;


            }
        }

        //yVelocity ���� ���� �ٿ��ش�. (�߷¿� ���ؼ�) //�̷��� �� ��� ���鿡 �ö��ִٰ� �ٴڿ� �������� Ȯ �������� �ȴ�. 
        yVelocity += gravity * Time.deltaTime;

        //dir�� y���� yVelocity�� �����Ѵ�. 
        dir.y = yVelocity;


        //3. �� �������� ��� �̵��ϰ� �ʹ�. 
        //�̵� ���� p = p0 + vt
        //transform.position = transform.position + dir * 5 * Time.deltaTime;
        cc.Move(dir * speed * Time.deltaTime);

    }
}
