using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponeSwitcher : MonoBehaviour
{
    public GameObject[] weapons;
    private int selectedWeapon = 0;


    void Start()
    {
        SelectWeapon();
    }

    // Update is called once per frame
    void Update()
    {
        int previousSelectedWeapon = selectedWeapon;

        // 마우스 휠 입력에 따라 무기 전환
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            selectedWeapon = (selectedWeapon + 1) % weapons.Length;
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            selectedWeapon--;
            if (selectedWeapon < 0)
            {
                selectedWeapon = weapons.Length - 1;
            }
        }

        // 선택된 무기가 변경된 경우 무기 전환
        if (previousSelectedWeapon != selectedWeapon)
        {
            SelectWeapon();
        }
    }

    void SelectWeapon()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].SetActive(i == selectedWeapon);
        }
    }
}
 
