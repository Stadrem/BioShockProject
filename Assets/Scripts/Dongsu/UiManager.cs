using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public static UiManager instance;

    public int[] boxList = new int[2];

    //1번 힐, 2번 마나, 3번 총알, 4번 달러 아이콘
    public List<Sprite> spriteList = new List<Sprite>();

    //아이콘들어갈 ui
    public List<Image> imgList = new List<Image>();

    //1번 힐, 2번 마나, 3번 총알, 4번 달러
    public int[] keepItems = new int[] { 0, 0, 0, 0 };

    public Sprite originSprite;

    Image HPGauge;

    public GameObject RootUi;
    bool rootUiOn = false;

    int maxHP;

    float currentHP;

    public LayerMask layerMask;

    public int maxItems = 3;

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
        HPGauge = temp.GetComponent<Image>();
        RootUi = GameObject.Find("RootUi");
        RootUi.SetActive(false);

        maxHP = GameManager.instance.HP;
        currentHP = maxHP;
    }

    // Update is called once per frame
    void Update()
    {
        // 카메라의 정면 방향으로 Ray를 쏩니다.
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;

        // Ray가 충돌했는지 확인합니다.
        if (Physics.Raycast(ray, out hit, 3.0f, layerMask))
        {
            // 충돌한 오브젝트가 itemBox 컴포넌트를 가지고 있는지 확인합니다.
            ItemBoxRoot itemBoxRoot = hit.collider.GetComponent<ItemBoxRoot>();
            if (itemBoxRoot != null)
            {
                // itemBox 컴포넌트를 실행합니다.
                RootUi.SetActive(true);
                rootUiOn = true;

                //아이템 박스 오픈
                itemBoxRoot.itemView();

                //아이템 획득
                if (Input.GetButtonDown("Get") && rootUiOn == true)
                {
                    BoxListRefresh();
                    itemBoxRoot.GetItem();
                }
            }
            else
            {
                RootUi.SetActive(false);
                rootUiOn = false;
            }
        }
        else
        {
            for(int i =0; i < imgList.Count; i++)
            {
                imgList[i].gameObject.SetActive(false);
            }
            RootUi.SetActive(false);
            rootUiOn = false;
        }
    }

    public void HPRefresh(int i)
    {
        currentHP = i;
        HPGauge.fillAmount = currentHP * 0.1f;
    }

    void BoxListRefresh()
    {
        for(int i = 0; i < 3; i++)
        {
            imgList[i].sprite = originSprite;
        }
    }
}
