using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public Sprite originEffect;
    public Sprite selectEffect;
    int selectNum = 0;
    //int beforeNum = 0;
    float whatUpDown = 0;

    private void Start()
    {
        Image storeSprite = UiManager.instance.storeList[selectNum].GetComponent<Image>();
        storeSprite.sprite = selectEffect;
    }

    private void OnEnable()
    {
        Time.timeScale = 0;
        UiManager.instance.dollarUi.SetActive(true);
    }

    void Update()
    {
        //키 입력
        if (Input.GetButtonDown("Vertical"))
        {
            //위아래 감지
            whatUpDown = Input.GetAxisRaw("Vertical");

            SoundManager.instance.SelectSound();

            // 아래로 이동할 때 (whatUpDown이 -1일 때)
            if (whatUpDown == -1)
            {
                // 현재 선택된 항목의 이미지를 원래대로 되돌림
                Image storeSprite = UiManager.instance.storeList[selectNum].GetComponent<Image>();
                storeSprite.sprite = originEffect;

                // selectNum을 1 증가시켜 다음 항목으로 이동
                selectNum++;

                // selectNum이 5를 초과하면 첫 번째 항목(0)으로 되돌림
                if (selectNum > 4)
                {
                    selectNum = 0;
                }

                // 새로운 선택된 항목의 이미지를 선택된 상태로 변경
                storeSprite = UiManager.instance.storeList[selectNum].GetComponent<Image>();
                storeSprite.sprite = selectEffect;
            }
            // 위로 이동할 때 (whatUpDown이 1일 때)
            else if (whatUpDown == 1)
            {
                // 현재 선택된 항목의 이미지를 원래대로 되돌림
                Image storeSprite = UiManager.instance.storeList[selectNum].GetComponent<Image>();
                storeSprite.sprite = originEffect;

                // selectNum을 1 감소시켜 이전 항목으로 이동
                selectNum--;

                // selectNum이 0보다 작으면 마지막 항목(5)으로 되돌림
                if (selectNum < 0)
                {
                    selectNum = 4;
                }

                // 새로운 선택된 항목의 이미지를 선택된 상태로 변경
                storeSprite = UiManager.instance.storeList[selectNum].GetComponent<Image>();
                storeSprite.sprite = selectEffect;
            }
        }

        //창 닫기
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UiManager.instance.ItemRefresh();

            Time.timeScale = 1;

            UiManager.instance.dollarUi.SetActive(false);

            gameObject.SetActive(false);
        }

        //구매
        if (Input.GetButtonDown("Get"))
        {
            //아이템 정보 가져오기
            ThisItemNum selectItem = UiManager.instance.storeList[selectNum].GetComponent<ThisItemNum>();

            //돈 부족하면 빠꾸
            if (UiManager.instance.keepItems[5] == 0 || UiManager.instance.keepItems[5] < selectItem.price)
            {
                SoundManager.instance.FailSound();

                UiManager.instance.Alret("달러가 부족합니다.");

                return;
            }
            //만약 구매하려는 아이템이 힐이나 마나 아이템이고, 그것이 9개 이상이면?
            else if (selectItem.itemNum == 4 || selectItem.itemNum == 6)
            {
                if(UiManager.instance.keepItems[selectItem.itemNum] >= 9)
                {
                    SoundManager.instance.FailSound();

                    UiManager.instance.Alret("해당 물품을 더이상 가질 수 없습니다.");

                    return;
                }
            }
            if(selectItem.itemNum == 2)
            {
                UiManager.instance.keepItems[selectItem.itemNum] += 10;
            }
            else
            {
                UiManager.instance.keepItems[selectItem.itemNum]++;
            }
            SoundManager.instance.PaySound();

            //돈은 충분한가?
            UiManager.instance.Alret(selectItem.itemName + "를 구매했습니다.");


            //달러 갱신
            UiManager.instance.keepItems[5] -= selectItem.price;

            UiManager.instance.dollarText.text = UiManager.instance.keepItems[5].ToString("D4");
        }
    }
}
