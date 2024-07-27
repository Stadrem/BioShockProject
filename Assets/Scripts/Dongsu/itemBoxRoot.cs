using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemBoxRoot : MonoBehaviour
{
    public List<int> itemList = new List<int>();

    int Max = 2;

    // Start is called before the first frame update
    void Start()
    {
        Max = UiManager.instance.maxItems;

        for (int i = 0; i < Max; i++)
        {
            if (itemList.Count == 3)
            {
                break;
            }
            int j = Random.Range(0, Max + 1);

            itemList.Add(j);

            if (Random.Range(0, 10) >= 8)
            {
                itemList.RemoveAt(itemList.Count - 1); // 마지막 아이템 제거
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void itemView()
    {
        for (int h = 0; h < itemList.Count; h++)
        {
            UiManager.instance.imgList[h].gameObject.SetActive(true);
        }
        for (int i = 0; i < itemList.Count; i++)
        {
            if (itemList != null)
            {
                if (UiManager.instance != null)
                {
                    UiManager.instance.imgList[i].sprite = UiManager.instance.spriteList[itemList[i]];
                }
            }
        }
    }
}
