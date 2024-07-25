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

        // ���콺 �� �Է¿� ���� ���� ��ȯ
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

        // ���õ� ���Ⱑ ����� ��� ���� ��ȯ
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
 
