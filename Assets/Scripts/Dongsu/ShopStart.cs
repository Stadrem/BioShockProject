using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopStart : MonoBehaviour
{
    public GameObject Ui;
    bool contact = false;

    private void Update()
    {
        if (Input.GetButtonDown("Get") && contact == true)
        {
            UiManager.instance.shopUi.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //상점 접촉
        if (other.transform.CompareTag("Player"))
        {
            contact = true;
            Ui.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //상점 퇴장
        if (other.transform.CompareTag("Player"))
        {
            contact = false;
            Ui.SetActive(false);
        }
    }
}
