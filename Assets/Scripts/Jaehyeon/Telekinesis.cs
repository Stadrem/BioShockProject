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
    public int weaponeIndex = 0;
    public GameObject effectObject; // 효과 오브젝트
    public float fireRate = 0.1f; // 발사 간격 (자동 발사용)
    private float lastFireTime = 0f;
    Animator anim;
    GameObject whatObjectTag;
    //public ParticleSystem effectPS;

    // 사운드 구현
    public AudioClip grabSound;
    public AudioClip throwSound;
    private AudioSource audioSource;

    private void Start()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
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
            if (UiManager.instance.Reload(weaponeIndex))
            {
                //UiManager.instance.Reload(weaponeIndex);
                anim.SetTrigger("RELOAD");
                
            }
            UseManaItem();
            return;
        }

        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            anim.SetFloat("WALK_AND_IDLE", 1, 0.25f * 0.3f, Time.deltaTime);
        }
        else
        {
            anim.SetFloat("WALK_AND_IDLE", 0, 0.25f * 0.3f, Time.deltaTime);

        }


        if (Input.GetButton("Fire1") && Time.time - lastFireTime >= fireRate)
        {

            if (GameManager.instance.HP <= 0)
            {
                Debug.Log("플레이어 HP가 0, 공격할 수 없습니다.");
                return;
            }
            // 만약 오브젝트를 당긴다면
            if (!grab)
            {
                if (TryUseMana())
                {
                    
                    SucGrabObject();
                    if (effectObject != null)
                    {
                        StartCoroutine(EffectPopUP()); // 이펙트를 보이게 함
                        
                    }
                    GameManager.instance.TeleEffect();
                    PlaySound(grabSound); // 소리 재생
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
                            StartCoroutine(EffectPopUP()); // 이펙트를 보이게 함
                        }
                        GameManager.instance.TeleEffect();
                        PlaySound(grabSound); // 소리 재생
                    }
                }
            }
            else
            {
                SucThrowObject();
                grab = false;
                StartCoroutine(EffectPopUP());
                GameManager.instance.TeleEffect();
                PlaySound(throwSound); // 소리 재생
            }
            
            lastFireTime = Time.time;
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
        if (UiManager.instance.keepItems[4] != 0)
        {
            anim.SetTrigger("RELOAD");
        }
        UiManager.instance.UseMana();
    }

    public void SucGrabObject()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;
        anim.SetTrigger("ATTACK");

        if (Physics.Raycast(ray, out hit, telekinesisRange, layerMask))
        {
            if (hit.collider != null)
            {
                whatObjectTag = hit.transform.gameObject;

                grabbedObject = hit.transform;

                Collider col = grabbedObject.GetComponent<Collider>();
                col.enabled = false;

                Rigidbody rb = grabbedObject.GetComponent<Rigidbody>();
                if (rb == null)
                {
                    rb = grabbedObject.gameObject.AddComponent<Rigidbody>();
                }

                Rigidbody[] allRb = grabbedObject.GetComponentsInChildren<Rigidbody>();
                foreach (Rigidbody r in allRb)
                {
                    r.useGravity = false;
                    r.angularVelocity = Vector3.zero;
                }

                
                grab = true;
            }
        }
    }

    public void SucThrowObject()
    {
        anim.SetTrigger("ATTACK");
        if (grabbedObject == null)
        {
            grab = false;
            return;
        }

        Collider col = grabbedObject.GetComponent<Collider>();
        col.enabled = true;

        Rigidbody rb = grabbedObject.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = grabbedObject.gameObject.AddComponent<Rigidbody>();
        }

        Rigidbody[] allRb = grabbedObject.GetComponentsInChildren<Rigidbody>();
        foreach(Rigidbody r in allRb)
        
        {
            r.useGravity = true;
            r.velocity = transform.forward * telekinesisForce;
        }

        //rb.useGravity = true;        
        //rb.velocity = transform.forward * telekinesisForce;

        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, telekinesisRange, throwdamMask))
        {
            Debug.Log(hitInfo.transform.name);
            if (hitInfo.collider.CompareTag("Enemy") || hitInfo.collider.CompareTag("Boss") && !whatObjectTag.CompareTag("Bomb"))
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
    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    IEnumerator EffectPopUP()
    {
        effectObject.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        effectObject.SetActive(false);
    }
}
