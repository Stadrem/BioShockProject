using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemBoxRoot : MonoBehaviour
{
    //0번 빈칸, 1번 권총탄, 2번 기관총, 3번 샷건, 4번 마나, 5번 달러, 6번 힐
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

        //아이템 리스트에 등록된 최대 갯수 만큼 루프
        for (int i = 1; i < Max; i++)
        {
            //보유할 최대 아이템은 3개
            if (itemList.Count == 3)
            {
                //3개 채워지면 강제 종료
                break;
            }
            //0~최대 아이템 index 사이에서 랜덤으로 값 생성
            int j = Random.Range(1, Max);

            // 제외할 인덱스가 아니면 추가
            if (!excludedIndices.Contains(j))
            {
                //무작위 추가. 동일한 아이템도 등장 가능
                itemList.Add(j);

                //추가적인 무작위성을 위해 20% 확률로 날려버림
                if (Random.Range(0, 10) >= 8)
                {
                    // 마지막 아이템 제거
                    itemList.RemoveAt(itemList.Count - 1);
                }
            }
        }
    }

    //ui 매니저에 현재 아이템 박스가 보유한 아이템들 보여주는 함수
    public void itemView()
    {
        //아이템 리스트 3칸에 정보 채워넣음
        for (int h = 0; h < itemList.Count; h++)
        {
            UiManager.instance.imgList[h].gameObject.SetActive(true);
            UiManager.instance.imgList[h].sprite = UiManager.instance.spriteList[itemList[h]];
        }

    }

    //아이템 획득 시 순차적으로 획득 및 제거
    public void GetItem()
    {
        //아이템이 비어있지 않다면
        if (itemList != null)
        {
            //상자에 등록된 아이템 갯수가 0보다 작음
            if (itemList.Count <= 0)
            {
                //그냥 빈칸만 보여줄거임
                itemView();
            }
            else
            {
                //몇개 주워갈지에 대한 임시 변수
                int tempNum = 0;

                //이게 무슨 아이템 이름인지?
                string tempName = UiManager.instance.nameList[itemList[0]];

                //달러면 2~45개 획득
                if (itemList[0] == 5)
                {
                    tempNum = Random.Range(2, 46);

                    UiManager.instance.keepItems[5] += tempNum;

                    //달러 전용 팝업
                    StartCoroutine(DollarGet());
                }
                //총알이면 3~10발
                else if (itemList[0] == 1 || itemList[0] == 2 || itemList[0] == 3)
                {
                    tempNum = Random.Range(3, 11);

                    UiManager.instance.keepItems[itemList[0]] += tempNum;
                }
                //그 외의 모든 아이템은 1개
                else
                {
                    tempNum = 1;

                    UiManager.instance.keepItems[itemList[0]] += tempNum;
                }
                //아이템 획득 알림
                StartCoroutine(ItemGetAlret(tempName, tempNum));

                //첫번째 배열 제거
                itemList.RemoveAt(0);

                //획득 후 남은 아이템 보여줌
                itemView();
            }
        }
        //보유 아이템 새로고침 및 사운드 재생
        UiManager.instance.ItemRefresh();
        SoundManager.instance.RootSound();
    }

    //달러 획득 팝업
    IEnumerator DollarGet()
    {
        //현재 숫자 자릿수에 따라서 0을 더 붙여줌 (D4)
        UiManager.instance.dollarText.text = UiManager.instance.keepItems[5].ToString("D4");

        //팝업 딸깍
        UiManager.instance.dollarUi.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        UiManager.instance.dollarUi.SetActive(false);
    }

    //아이템 갯수 알림 팝업
    IEnumerator ItemGetAlret(string name, int num)
    {
        string sum = "획득: " + name + " ( " + num + " )";
        UiManager.instance.getAlret.text = sum;

        //팝업 딸깍
        UiManager.instance.getAlret.gameObject.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        UiManager.instance.getAlret.gameObject.SetActive(false);
    }
}
