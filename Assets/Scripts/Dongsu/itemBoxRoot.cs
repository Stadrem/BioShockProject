using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemBoxRoot : MonoBehaviour
{
    //아이템 박스에 들어갈 리스트
    public List<int> itemList = new List<int>();

    //현재 게임내에 구현된 아이템 갯수
    int Max;

    // 제외할 아이템 인덱스 리스트
    public List<int> excludedIndices = new List<int>();

    void Start()
    {
        //ui 매니저에서 아이템 갯수 가져오기
        Max = UiManager.instance.maxItems;

        //아이템 리스트에 등록된 초대 갯수 만큼 루프
        for (int i = 0; i < Max; i++)
        {
            //보유할 최대 아이템은 3개
            if (itemList.Count == 3)
            {
                //3개 채워지면 강제 종료
                break;
            }
            //0~최대 아이템 index 사이에서 랜덤으로 값 생성
            int j = Random.Range(0, Max + 1);

            // 제외할 인덱스가 아니면 추가
            if (!excludedIndices.Contains(j))
            {
                //무작위 추가. 동일한 아이템도 등장 가능
                itemList.Add(j);

                //추가적인 무작위성을 위해 10% 확률로 날려버림
                if (Random.Range(0, 10) >= 9)
                {
                    itemList.RemoveAt(itemList.Count - 1); // 마지막 아이템 제거
                }
            }
        }
    }

    //ui 매니저에 현재 아이템 박스가 보유한 아이템들 보여주는 함수
    public void itemView()
    {
        for (int h = 0; h < itemList.Count; h++)
        {
            UiManager.instance.imgList[h].gameObject.SetActive(true);
            UiManager.instance.imgList[h].sprite = UiManager.instance.spriteList[itemList[h]];
        }

    }

    //아이템 획득 시 순차적으로 제거
    public void GetItem()
    {
        if (itemList != null)
        {
            if (itemList.Count <= 0)
            {
                itemView();
            }
            else
            {
                UiManager.instance.keepItems[itemList[0]] += 1;
                itemList.RemoveAt(0);
                itemView();
            }
        }
        UiManager.instance.ItemRefresh();
    }
}
