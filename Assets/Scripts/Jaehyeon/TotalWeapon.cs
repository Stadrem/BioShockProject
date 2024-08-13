using System.Collections;
using UnityEngine;

public class TotalWeapon : MonoBehaviour
{
    public LayerMask layerMask;
    public GameObject effectPrefab; // 무기 또는 마법 효과 프리팹
    public GameObject fireEffect;
    public float attackRange = 100f; // 공격 범위
    public int damage = 5; // 공격 데미지
    public string type = "?";

    // 총기류 관련 변수
    public bool AutoFire = false; // 자동 발사 여부 (기관총 등)
    public float fireRate = 0.1f; // 발사 간격 (자동 발사용)
    public int magazineSize = 30; // 탄창 크기
    public bool needMag = true; // 탄창이 필요한지 여부

    public int weaponeIndex = 0;

    private float lastFireTime = 0f;

    public float Rebound = 2.0f;
    public GameObject muzzleFlashPrefab; // 머즐 플래시 프리팹
    public Transform muzzleFlashPosition; // 머즐 플래시 위치
    public bool isSwitching = false;

    // 마나 아이템 사용시 소모
    public int manaCost = 1; // 마법 사용 시 소모되는 마나

    public bool isMagic = false;

    public bool isShockandFire = false;

    Animator anim;

    bool isReloading = false; // 장전 상태를 추적
    public bool IsReloading()
    {
        return isReloading;
    }

    public bool isAttacking = false;

    //사운드 구현
    public AudioClip AttackSound;
    public AudioClip ReloadSound;
    private AudioSource audioSource;


    void Start()
    {
        anim = GetComponent<Animator>();
        Cursor.lockState = CursorLockMode.Locked;
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {



        if (Input.GetKeyDown(KeyCode.R))
        {
            if (isMagic)
            {
                UseManaItem();
            }
            else
            {
                Reload();
            }
            return;
        }

        if (isReloading || isSwitching) // 장전 중에는 발사 불가능
            return;

        if (isMagic)
        {
            HandleMagicFire();
        }
        else
        {
            HandleWeaponFire();
        }

        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            anim.SetFloat("WALK_AND_IDLE", 1, 0.25f * 0.3f, Time.deltaTime);
        }
        else
        {
            anim.SetFloat("WALK_AND_IDLE", 0, 0.25f * 0.3f, Time.deltaTime);
        }
    }

    void HandleWeaponFire()
    {
        if (AutoFire)
        {
            if (Input.GetButton("Fire1") && Time.time - lastFireTime >= fireRate)
            {
                if (!needMag || UiManager.instance.BulletShoot())
                {
                    Shoot();
                    lastFireTime = Time.time;
                }
                else
                {
                    Debug.Log("탄약 부족. 재장전 필요.");
                }
            }
        }
        else
        {
            if (Input.GetButtonDown("Fire1"))
            {
                if (!needMag || UiManager.instance.BulletShoot())
                {
                    Shoot();
                }
                else
                {
                    Debug.Log("탄약 부족. 재장전 필요.");
                }
            }
        }
    }

    void HandleMagicFire()
    {
        if (Input.GetButtonDown("Fire1") && Time.time - lastFireTime >= fireRate)
        {
            if (TryUseMana())
            {
                UiManager.instance.ManaRefresh(manaCost);
                Shoot();
                lastFireTime = Time.time;
                if (!TryUseMana())
                {
                    UiManager.instance.UseMana(); // 마나 아이템 자동 사용
                }
            }
            else
            {
                UiManager.instance.UseMana(); // 마나 아이템 자동 사용
            }
        }
    }

    bool TryUseMana()
    {
        if (UiManager.instance.currentMana < 0)
        {
            return false;
        }
        else
        {
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

    public void Shoot()
    {

        if (GameManager.instance.HP <= 0 || isReloading || isSwitching)
        {
            Debug.Log("플레이어 HP가 0입니다. 공격할 수 없습니다.");
            return;
        }

        isAttacking = true;  // 공격 시작
        anim.SetTrigger("ATTACK");
        StartCoroutine(EndAttack());



        // 발사 소리 재생
        if (AttackSound != null && audioSource != null)
        {
            //audioSource.Play();
            //audioSource.time = 0.5f;
            audioSource.PlayOneShot(AttackSound);
        }


        if (effectPrefab == null)
        {
            Debug.LogError("Inspector창에 무기상태가 안들어감");
            return;
        }

        // Raycast를 이용해 적 감지 및 데미지 적용
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, attackRange, layerMask))
        {
            Debug.Log(hitInfo.transform.name);
            if (hitInfo.collider.CompareTag("Enemy"))
            {
                Damaged damaged = hitInfo.collider.GetComponent<Damaged>();
                damaged.Damage(damage, type);
            }
            else if (hitInfo.collider.CompareTag("Boss"))
            {
                BossDamaged bossDamaged = hitInfo.collider.GetComponent<BossDamaged>();
                bossDamaged.Damaged(damage, type);
            }
            // 파편 효과 생성
            GameObject bulletImpact = Instantiate(effectPrefab);

            if (isShockandFire == true)
            {
                if (hitInfo.transform.CompareTag("Enemy") || hitInfo.transform.CompareTag("Boss"))
                {
                    GameObject fire = Instantiate(fireEffect);

                    fire.transform.position = hitInfo.transform.position;
                    fire.transform.parent = hitInfo.transform;

                    Destroy(fire, 2f);
                }
            }

            bulletImpact.transform.position = hitInfo.point;
            if (isMagic == true)
            {
                bulletImpact.transform.forward = transform.forward;
            }
            else
            {
                bulletImpact.transform.forward = hitInfo.normal;
            }

            Destroy(bulletImpact, 1); // 2초 뒤에 파괴

            
        }

        



        // 머즐 플래시 생성
        if (muzzleFlashPrefab != null && muzzleFlashPosition != null)
        {
            GameObject muzzleFlash = Instantiate(muzzleFlashPrefab, muzzleFlashPosition.position, muzzleFlashPosition.rotation);
           Destroy(muzzleFlash, 0.1f); // 0.1초 뒤에 파괴
        }

        // 반동 효과 적용
        ApplyRebound();

    }



    void Reload()
    {
        if (needMag && !isReloading)
        {
            // 현재 탄창이 가득 차 있는지 확인
            bool canReload = UiManager.instance.Reload(weaponeIndex);

            if (canReload)
            {

                anim.SetTrigger("RELOAD");
                StartCoroutine(ReloadCoroutine());

                // 탄창이 가득 차지 않았을 때만 소리 재생
                if (ReloadSound != null && audioSource != null)
                {
                    audioSource.PlayOneShot(ReloadSound);
                }
            }
            else
            {
                Debug.Log("탄창이 이미 가득 찼습니다. 장전 소리를 재생하지 않습니다.");
            }
        }
    }


    IEnumerator ReloadCoroutine()
    {
        isReloading = true;
        yield return new WaitForSeconds(2f); // 2초 딜레이
        isReloading = false;
    }
    void ApplyRebound()
    {
        isRebound = 1;
    }

    int isRebound;
    private void LateUpdate()
    {
        if(isRebound > 0)
        {
            Camera.main.transform.Rotate(-Rebound, 0, 0);
            isRebound++;
            isRebound %= 3;            
        }        
    }

    private IEnumerator EndAttack()
    {
        yield return new WaitForSeconds(1f); // 예를 들어 1초 뒤에 공격이 끝난다고 가정
        isAttacking = false;
    }

}


