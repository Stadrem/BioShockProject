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

    //0번 번개, 1번 불꽃, 2번 염동력
    public List<Sprite> spriteMagicList = new List<Sprite>();

    //0번 힐, 1번 권총탄, 2번 기관총, 3번 샷건, 4번 마나, 5번 달러, 6번 힐
    public int[] keepItems = new int[] { 0, 0, 0, 0, 0, 0, 0 };

    //계획된 아이템 갯수
    public int maxItems = 8;

    //HP 게이지 갱신
    Image hpGauge;
    public float currentHP;

    //Mana 게이지 갱신
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
    int[] weaponeMagazine = new int[] { 0, 0, 0, 0 };
    int currentWeapone = 1;

    bool currentWahtMagic = false;

    public GameObject dialogueUi;
    public Text dialougeText;

    public RectTransform crossHair;

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

        //DialoguePopUp("이 지역에 리틀 시스터가 있습니다.\r\n\r\n리틀 시스터를 구원하려면 먼저 빅 대디를 처리해야합니다.", 5.0f);
    }

    // Update is called once per frame
    void Update()
    {
        ItemBox();
        if (Input.GetButtonDown("Heal"))
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
        GameManager.instance.HP = i;

        hpGauge.fillAmount = GameManager.instance.HP * 0.1f;

        if (hpGauge.fillAmount >= 1)
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
        if(keepItems[6] > 9)
        {
            keepItems[6] = 9;
        }
        if (keepItems[4] > 9)
        {
            keepItems[4] = 9;
        }

        healItem.text = keepItems[6].ToString();
        manaItem.text = keepItems[4].ToString();
        if(currentWeapone != 0 && currentWahtMagic == false)
        {
            bulletCurrentText.text = weaponeMagazine[currentWeapone].ToString();
            bulletMaxText.text = keepItems[currentWeapone].ToString();
        }
        else if(currentWeapone == 0)
        {
            bulletCurrentText.text = " ";
            bulletMaxText.text = " ";
        }
    }

    void UseHeal()
    {
        if (keepItems[6] > 0)
        {
            keepItems[6] -= 1;
            ItemRefresh();
            GameManager.instance.HP = GameManager.instance.maxHP;
            HPRefresh(GameManager.instance.maxHP);
        }
        else if(GameManager.instance.HP == GameManager.instance.maxHP)
        {
            return;
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

    public bool Reload(int weapone)
    {
        currentWeapone = weapone;

        if (currentWeapone == 1)
        {
            needMagazine = 8;
        }
        else if (currentWeapone == 2)
        {
            needMagazine = 30;
        }
        else if (currentWeapone == 3)
        {
            needMagazine = 4;
        }
        else
        {
            return false;
        }

        if(weaponeMagazine[currentWeapone] == needMagazine)
        {
            return false;
        }

        //갯수가 없으면 리턴걸어줘야함
        if (keepItems[currentWeapone] == 0)
        {
            alretAnim.SetTrigger("Alret");
            return false;
        }

        //약간 부족하면 필요한 만큼 계산
        else if (keepItems[currentWeapone] < needMagazine)
        {
            //필요한 장전 총알량 계산
            needMagazine = needMagazine - weaponeMagazine[currentWeapone];
            if(keepItems[currentWeapone] < needMagazine)
            {
                //그냥 다 떄려넣기
                weaponeMagazine[currentWeapone] += keepItems[currentWeapone];

                keepItems[currentWeapone] = 0;
            }
            else
            {
                //가방에서 필요한 장전량을 뺌
                keepItems[currentWeapone] -= needMagazine;

                //현재 탄환에 필요한 장전량 추가
                weaponeMagazine[currentWeapone] += needMagazine;
            }
        }

        //일반적인 재장전
        else
        {
            //장전된 탄환을 가방으로 돌려보냄
            keepItems[currentWeapone] += weaponeMagazine[currentWeapone];

            //장전된 탄환 초기화
            weaponeMagazine[currentWeapone] = 0;

            //요청 탄환 수 만큼 보유량 제거
            keepItems[currentWeapone] -= needMagazine;

            //장전
            weaponeMagazine[currentWeapone] = needMagazine;
        }

        //갱신
        bulletCurrentText.text = weaponeMagazine[currentWeapone].ToString();
        bulletMaxText.text = keepItems[currentWeapone].ToString();
        return true;
    }

    public void WeaponeChange(int weapone)
    {
        switch (weapone)
        {
            case 0:
                weaponeName.text = "Spanner";
                bulletCurrentText.text = " ";
                bulletMaxText.text = " ";
                crossHair.localScale = new Vector3(0.5f, 0.5f, 1);
                break;
            case 1:
                weaponeName.text = "Revolver";
                crossHair.localScale = new Vector3(1, 1, 1);
                break;
            case 2:
                weaponeName.text = "Thompson";
                crossHair.localScale = new Vector3(1.5f, 1.5f, 1);
                break;
            case 3:
                weaponeName.text = "Shot Gun";
                crossHair.localScale = new Vector3(2f, 2f, 1);
                break;
        }
        weaponeBulletIcon.sprite = spriteList[weapone];

        currentWeapone = weapone;
        if (weapone != 0)
        {
            bulletCurrentText.text = weaponeMagazine[currentWeapone].ToString();
            bulletMaxText.text = keepItems[currentWeapone].ToString();
        }

    }

    public void MagicChange(int magic)
    {
        crossHair.localScale = new Vector3(0.2f, 0.2f, 1);
        switch (magic)
        {
            case 0:
                weaponeName.text = "Shock";
                break;
            case 1:
                weaponeName.text = "Fire";
                break;
            case 2:
                weaponeName.text = "Telekinesis";
                break;
        }
        bulletCurrentText.text = " ";
        bulletMaxText.text = " ";
        weaponeBulletIcon.sprite = spriteMagicList[magic];
    }

    public void Switcher(bool i)
    {
        currentWahtMagic = i;
    }

    public void DialoguePopUp(string text, float time)
    {
        StartCoroutine(DialogueDelay(text, time));
    }

    IEnumerator DialogueDelay(string text, float time)
    {
        Time.timeScale = 0;

        dialougeText.text = text;

        dialogueUi.SetActive(true);

        yield return new WaitForSecondsRealtime(time);

        dialogueUi.SetActive(false);

        Time.timeScale = 1;
    }
}
