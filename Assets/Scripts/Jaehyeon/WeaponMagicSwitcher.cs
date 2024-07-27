using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMagicSwitcher : MonoBehaviour
{
    public GameObject[] weapons; // 무기 배열 (오른손)
    public GameObject magic; // 마법 (왼손)
    public GameObject rightArm;
    public Transform rightHand; // 오른손 위치
    public Transform leftHand; // 왼손 위치

    private int selectedWeapon = 0;
    private int selectedmagic = 0;
    private bool isMagicActive = false; // 현재 마법 모드인지 여부

    void Start()
    {
        SelectWeaponOrMagic();
    }

    void Update()
    {
        // 마우스 휠로 무기 전환
        if (!isMagicActive)
        {
            int previousSelectedWeapon = selectedWeapon;

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

            if (previousSelectedWeapon != selectedWeapon)
            {
                SelectWeapon();
            }
        }

        // 마우스 우클릭으로 무기와 마법 전환
        if (Input.GetButtonDown("Fire2")) // 우클릭
        {
            isMagicActive = !isMagicActive;
            SelectWeaponOrMagic();
        }

        // 마우스 좌클릭으로 무기 또는 마법 사용
        if (Input.GetButtonDown("Fire1")) // 좌클릭
        {
            if (isMagicActive)
            {
                // 마법 사용
               // magic.GetComponent<MagicShoot>().ShootMagic();
               magic.GetComponent<TotalWeapon>().Shoot();

            }
            else
            {
                // 무기 사용
                //weapons[selectedWeapon].GetComponent<WeaponBase>().Use();
                weapons[selectedWeapon].GetComponent<TotalWeapon>().Shoot();
            }
        }
    }

    void SelectWeapon()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].SetActive(i == selectedWeapon && !isMagicActive);
            if (i == selectedWeapon && !isMagicActive)
            {
                weapons[i].transform.SetParent(rightHand);
                weapons[i].transform.localPosition = Vector3.zero;
                weapons[i].transform.localRotation = Quaternion.identity;
            }
        }
    }

/*    void Selectmagic()
    {
        for (int i = 0; i < magic.Length; i++)
        {
            magic[i].SetActive(i == selectedWeapon && !isMagicActive);
            if (i == selectedWeapon && !isMagicActive)
            {
                magic[i].transform.SetParent(leftHand);
                magic[i].transform.localPosition = Vector3.zero;
                magic[i].transform.localRotation = Quaternion.identity;
            }
        }
    }*/

    void SelectWeaponOrMagic()
    {
        if (isMagicActive)
        {
            magic.SetActive(true);
            rightArm.SetActive(false);
            magic.transform.SetParent(leftHand);
            //magic.transform.localPosition = Vector3.zero;
            //magic.transform.localRotation = Quaternion.identity;

            for (int i = 0; i < weapons.Length; i++)
            {
                weapons[i].SetActive(false);
            }
        }
        else
        {
            magic.SetActive(false);
            rightArm.SetActive(true);
            SelectWeapon();
        }
    }
}
