using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThisItemNum : MonoBehaviour
{
    public int itemNum = 0;
    public string itemName = "";
    public int price = 0;
    public Sprite icon;

    public Text originName;
    public Text originPrice;
    public Image originIcon;

    private void Start()
    {
        originName.text = itemName;
        originPrice.text = price.ToString();
        originIcon.sprite = icon;
    }
}
