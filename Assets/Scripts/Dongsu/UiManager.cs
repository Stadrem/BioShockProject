using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Searcher;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public static UiManager instance;

    //아이템 루팅
    public int[] boxList = new int[2];

    //아이콘들어갈 ui
    public List<Image> imgList = new List<Image>();

    //0번 힐, 1번 권총탄, 2번 기관총, 3번 샷건, 4번 마나, 5번 달러, 힐
    public List<Sprite> spriteList = new List<Sprite>();

    //0번 힐, 1번 권총탄, 2번 기관총, 3번 샷건, 4번 마나, 5번 달러, 6번 힐
    public int[] keepItems = new int[] { 0, 0, 0, 0, 0, 0, 0};

    //계획된 아이템 갯수
    public int maxItems = 8;

    //HP 게이지 갱신
    Image hpGauge;
    public float currentHP;

    //Mana 게이지 개잇ㄴ
    Image manaGauge;
    public float currentMana = 1;

    //아이템 루팅 관련
    public GameObject rootUi;
    public GameObject nameUi;
    public GameObject searchUi;
    public GameObject nameSpaceUi;
    public Sprite originSprite;
    public Image weaponeBulletIcon;
    public Text weaponeName;
    bool rootUiOn = false;

    //현재 아이템
    public Text healItem;
    public Text manaItem;

    //경고창
    public Text alretText;
    Animator alretAnim;

    //아이템 루팅 레이 판별
    public LayerMask layerMask;

    //탄창 관련
    //public int bulletCurrent = 0;
    //public int bulletMax = 0;
    public Text bulletCurrentText;
    public Text bulletMaxText;

    int needMagazine;
    //1번 권총탄, 2번 기관총, 3번 샷건, 0번 빈칸
    int[] weaponeMagazine = new int[] {0, 0, 0, 0 };
    int currentWeapone = 1;

    void Awake()
    {
        //instance 값이 null이면
        if (instance == null)
        {
            //이 스크립트를 instance에 담음
            instance = this;

            //씬 전환해도 유지하는 코드
            DontDestroyOnLoad(gameObject);
        }
        //이미 instance에 무언가 값이 들어있다면?
        else
        {
            //의도치 않은 중복 적용일 태니 이 게임 오브젝트 파괴.
            Destroy(gameObject);
        }
    }

    void Start()
    {
        GameObject temp = GameObject.Find("HPRed");
        hpGauge = temp.GetComponent<Image>();
        temp = GameObject.Find("ManaBlue");
        manaGauge = temp.GetComponent<Image>();
        currentHP = GameManager.instance.HP;
        alretAnim = alretText.GetComponent<Animator>();

        ItemRefresh();
        WeaponeChange(0);
    }

    // Update is called once per frame
    void Update()
    {
        ItemBox();
        if(Input.GetButtonDown("Heal"))
        {
            UseHeal();
            ItemRefresh();
        }
    }

    void ItemBox()
    {
        // 카메라의 정면 방향으로 Ray를 쏩니다.
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;

        // Ray가 충돌했는지 확인합니다.
        if (Physics.Raycast(ray, out hit, 3.0f, layerMask))
        {
            //타겟 이름을 받아옴
            nameUi.SetActive(true);
            Text nameText = nameUi.GetComponent<Text>();
            nameText.text = hit.transform.root.name;

            // 충돌한 오브젝트가 itemBox 컴포넌트를 가지고 있는지 확인합니다.
            ItemBoxRoot itemBoxRoot = hit.collider.GetComponent<ItemBoxRoot>();

            //박스가 비어있지 않다면
            if (itemBoxRoot.itemList.Count > 0)
            {
                searchUi.SetActive(true);

                if (Input.GetButtonDown("Get") && rootUiOn)
                {
                    BoxListRefresh();
                    itemBoxRoot.GetItem();
                    ItemRefresh();
                }

                if (Input.GetButtonDown("Get") && !rootUiOn)
                {
                    print("아이템 박스 오픈");
                    nameUi.SetActive(false);
                    searchUi.SetActive(false);
                    nameSpaceUi.SetActive(false);

                    // UI 표시
                    rootUi.SetActive(true);
                    rootUiOn = true;

                    //아이템 박스 정보 가져오기
                    itemBoxRoot.itemView();
                }
            }
            else
            {
                nameSpaceUi.SetActive(true);
                nameUi.SetActive(true);
                searchUi.SetActive(false);
                rootUi.SetActive(false);
                rootUiOn = false;
            }
        }
        else
        {
            //탐색 종료
            nameUi.SetActive(false);
            searchUi.SetActive(false);

            for (int i = 0; i < imgList.Count; i++)
            {
                imgList[i].gameObject.SetActive(false);
            }

            rootUi.SetActive(false);
            nameSpaceUi.SetActive(true);
            rootUiOn = false;
        }
    }

    public void HPRefresh(int i)
    {
        currentHP = i;

        hpGauge.fillAmount = currentHP * 0.1f;

        if(hpGauge.fillAmount >= 1)
        {
            hpGauge.fillAmount = 1;
        }
    }

    public void ManaRefresh(float i)
    {
        i = i * 0.1f;
        currentMana -= i;

        manaGauge.fillAmount = currentMana;

        if (manaGauge.fillAmount >= 1)
        {
            manaGauge.fillAmount = 1;
        }
    }

    void BoxListRefresh()
    {
        for (int i = 0; i < 3; i++)
        {
            imgList[i].sprite = originSprite;
        }
    }

    public void ItemRefresh()
    {
        healItem.text = keepItems[0].ToString();
        manaItem.text = keepItems[4].ToString();
        bulletCurrentText.text = weaponeMagazine[currentWeapone].ToString();
        bulletMaxText.text = keepItems[currentWeapone].ToString();
    }

    void UseHeal()
    {
        if(keepItems[6] > 0)
        {
            keepItems[6] -= 1;
            ItemRefresh();
            currentHP = GameManager.instance.maxHP;
            HPRefresh(0);
        }
        else
        {
            alretAnim.SetTrigger("Alret");
        }
    }

    public void UseMana()
    {
        if (keepItems[4] > 0)
        {
            keepItems[4] -= 1;
            ItemRefresh();
            currentMana = 1;
            ManaRefresh(0);
        }
        else
        {
            alretAnim.SetTrigger("Alret");
        }
    }

    public bool BulletShoot()
    {
        if (weaponeMagazine[currentWeapone] == 0)
        {
            alretAnim.SetTrigger("Alret");
            return false;
        }
        weaponeMagazine[currentWeapone] -= 1;

        bulletCurrentText.text = weaponeMagazine[currentWeapone].ToString();

        return true;
    }

    public void Reload(int weapone)
    {
        currentWeapone = weapone;

        if(currentWeapone == 1)
        {
            needMagazine = 8;
        }
        else if(currentWeapone == 2)
        {
            needMagazine = 30;
        }
        else if (currentWeapone == 3)
        {
            needMagazine = 4;
        }
        else
        {
            return;
        }

        //갯수가 없으면 리턴걸어줘야함
        if (keepItems[currentWeapone] == 0)
        {
            alretAnim.SetTrigger("Alret");
            return;
        }
        //약간 부족하면 그냥 다 때려넣음
        else if (keepItems[currentWeapone] < needMagazine)
        {
            needMagazine = keepItems[currentWeapone];
        }

        //보유중인 탄환을 임시 저장소에 돌려보냄
        keepItems[currentWeapone] += weaponeMagazine[currentWeapone];

        //장전된 탄환 초기화
        weaponeMagazine[currentWeapone] = 0;

        //요청 탄환 수 만큼 보유량 제거
        keepItems[currentWeapone] -= needMagazine;

        //장전
        weaponeMagazine[currentWeapone] = needMagazine;

        //갱신
        bulletCurrentText.text = weaponeMagazine[currentWeapone].ToString();
        bulletMaxText.text = keepItems[currentWeapone].ToString();
    }

    public void WeaponeChange(int weapone)
    {
        
        if(weapone == 0)
        {
            weaponeName.text = "Spanner";
            weaponeBulletIcon.sprite = spriteList[0];

            currentWeapone = weapone;
            bulletCurrentText.text = " ";
            bulletMaxText.text = " ";
        }
        else
        {
            if(weapone == 1)
            {
                weaponeName.text = "Revolver";
            }
            else if(weapone == 2)
            {
                weaponeName.text = "Machine Gun";
            }
            else if (weapone == 3)
            {
                weaponeName.text = "ShotGun";
            }
            weaponeBulletIcon.sprite = spriteList[weapone];

            currentWeapone = weapone;
            bulletCurrentText.text = weaponeMagazine[currentWeapone].ToString();
            bulletMaxText.text = keepItems[currentWeapone].ToString();
        }
    }
}
