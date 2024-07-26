using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    public GameObject[] weapons; // ���� �迭
    private int selectedWeapon = 0;
    public Transform weaponHolder; // ���Ⱑ ������ ������ ��ġ

    void Start()
    {
        // ���� �迭�� ����ִ��� Ȯ��
        if (weapons.Length == 0)
        {
            Debug.LogError("WeaponSwitcher: No weapons assigned!");
            return;
        }
        SelectWeapon();
    }

    void Update()
    {
        // ���� �迭�� ����ִ� ��� Update ����
        if (weapons.Length == 0)
        {
            return;
        }

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
            if (i == selectedWeapon)
            {
                weapons[i].transform.SetParent(weaponHolder);
                weapons[i].transform.localPosition = Vector3.zero;
                weapons[i].transform.localRotation = Quaternion.identity;
            }
        }
    }
}
