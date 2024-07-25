using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    public GameObject[] weapons; // 무기 배열
    private int selectedWeapon = 0;
    public Transform weaponHolder; // 무기가 장착될 오른팔 위치

    void Start()
    {
        // 무기 배열이 비어있는지 확인
        if (weapons.Length == 0)
        {
            Debug.LogError("WeaponSwitcher: No weapons assigned!");
            return;
        }
        SelectWeapon();
    }

    void Update()
    {
        // 무기 배열이 비어있는 경우 Update 중지
        if (weapons.Length == 0)
        {
            return;
        }

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
            if (i == selectedWeapon)
            {
                weapons[i].transform.SetParent(weaponHolder);
                weapons[i].transform.localPosition = Vector3.zero;
                weapons[i].transform.localRotation = Quaternion.identity;
            }
        }
    }
}
