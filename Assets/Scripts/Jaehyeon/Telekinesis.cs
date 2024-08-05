using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Telekinesis : MonoBehaviour
{
    public LayerMask layerMask; //select
    public LayerMask throwdamMask; //enemy
    public float telekinesisRange = 100f;
    public float telekinesisForce = 10f;
    public float telekinesisGrabbi = 2f;
    private bool grab = false;
    private Transform grabbedObject;
    public int damage = 5; // 공격 데미지
    public string type = "?";
    public int manaCost = 1; // 염력 사용 시 소모되는 마나

    private GameObject effectObject; // 효과 오브젝트

    private void Start()
    {
        // 플레이어에 붙어 있는 이펙트 오브젝트를 찾아서 할당
        effectObject = GameObject.Find("CFXR Water Ripples");
        if (effectObject != null)
        {
            effectObject.SetActive(false); // 초기에는 비활성화 상태로 설정
        }
      
    }

    void Update()
    {
        // R키로 마나아이템 사용(마나가 0이되지 않았을때도 사용가능)
        if (Input.GetKeyDown(KeyCode.R))
        {
            UseManaItem();
            return;
        }

        if (Input.GetButtonDown("Fire1"))
        {
            // 만약 오브젝트를 당긴다면
            if (!grab)
            {
                if (TryUseMana())
                {
                    SucGrabObject();
                    if (effectObject != null)
                    {
                        effectObject.SetActive(true); // 이펙트를 보이게 함
                    }
                }
                else
                {
                    Debug.Log("마나 부족. 마나 아이템을 사용하여 회복 중...");
                    UiManager.instance.UseMana();
                    if (TryUseMana())
                    {
                        SucGrabObject();
                        if (effectObject != null)
                        {
                            effectObject.SetActive(true); // 이펙트를 보이게 함
                        }
                    }
                }
            }
            else
            {
                SucThrowObject();
                grab = false;
                if (effectObject != null)
                {
                    effectObject.SetActive(false); // 던질 때 이펙트를 숨김
                }
            }
        }

        if (grab && grabbedObject != null)
        {
            // 오브젝트를 플레이어 앞에 계속해서 유지
            grabbedObject.position = Vector3.Lerp(grabbedObject.position, Camera.main.transform.position + Camera.main.transform.forward * telekinesisGrabbi, Time.deltaTime * 10);
        }
    }

    bool TryUseMana()
    {
        if (UiManager.instance.currentMana < manaCost * 0.1f)
        {
            return false;
        }
        else
        {
            UiManager.instance.ManaRefresh(manaCost);
            return true;
        }
    }

    void UseManaItem()
    {
        UiManager.instance.UseMana();
    }

    void SucGrabObject()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, telekinesisRange, layerMask))
        {
            if (hit.collider != null)
            {
                grabbedObject = hit.transform;

                Rigidbody rb = grabbedObject.GetComponent<Rigidbody>();
                if (rb == null)
                {
                    rb = grabbedObject.gameObject.AddComponent<Rigidbody>();
                }

                rb.useGravity = false;
                grab = true;
            }
        }
    }

    void SucThrowObject()
    {
        if (grabbedObject == null)
        {
            grab = false;
            return;
        }

        Rigidbody rb = grabbedObject.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = grabbedObject.gameObject.AddComponent<Rigidbody>();
        }

        rb.useGravity = true;
        rb.velocity = transform.forward * telekinesisForce;

        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, telekinesisRange, throwdamMask))
        {
            Debug.Log(hitInfo.transform.name);
            if (hitInfo.collider.CompareTag("Enemy") || hitInfo.collider.CompareTag("Boss"))
            {
                Damaged damaged = hitInfo.collider.GetComponent<Damaged>();
                if (damaged != null)
                {
                    damaged.Damage(damage, type);
                }
                else
                {
                    BossDamaged bossDamaged = hitInfo.collider.GetComponent<BossDamaged>();
                    if (bossDamaged != null)
                    {
                        bossDamaged.Damaged(damage, type);
                    }
                }
            }
        }
    }
}
