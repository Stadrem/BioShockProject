using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Searcher;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public static UiManager instance;

    public int[] boxList = new int[2];

    //0번 힐, 1번 마나, 2번 총알, 3번 달러 아이콘
    public List<Sprite> spriteList = new List<Sprite>();

    //아이콘들어갈 ui
    public List<Image> imgList = new List<Image>();

    //0번 힐, 1번 마나, 2번 총알, 3번 달러
    public int[] keepItems = new int[] { 0, 0, 0, 0 };

    public Sprite originSprite;

    Image hpGauge;

    public GameObject rootUi;
    public GameObject nameUi;
    public GameObject searchUi;
    public GameObject nameSpaceUi;
    public Text healItem;
    public Text manaItem;
    public Text alretText;
    Animator alretAnim;
    bool rootUiOn = false;

    float currentHP;

    public LayerMask layerMask;

    public int maxItems = 3;

    float delayTime = 0;

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

    // Start is called before the first frame update
    void Start()
    {
        GameObject temp = GameObject.Find("HPRed");
        hpGauge = temp.GetComponent<Image>();
        //RootUi = GameObject.Find("RootUi");
        //RootUi.SetActive(false);

        currentHP = GameManager.instance.HP;
        alretAnim = alretText.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        ItemBox();
        if(Input.GetButtonDown("Heal"))
        {
            UseHeal();
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
        manaItem.text = keepItems[1].ToString();
    }

    void UseHeal()
    {
        if(keepItems[0] > 0)
        {
            keepItems[0] -= 1;
            ItemRefresh();
            GameManager.instance.Damaged(-1000);
        }
        else
        {
            alretAnim.SetTrigger("Alret");
        }
    }
}
