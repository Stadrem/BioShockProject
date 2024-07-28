using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMagicSwitcher : MonoBehaviour
{
    public GameObject[] weapons; // 무기 배열 (오른손)
    public GameObject[] magic; // 마법 (왼손)
    public GameObject rightArm;
    public GameObject leftArm;
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

            //마우스 휠 위로
            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                selectedWeapon = (selectedWeapon + 1) % weapons.Length;
            }
            //마우스 휠 아래로 
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
        else
        {
            // 마우스 휠로 마법 전환
            int previousSelectedMagic = selectedmagic;

            //마우스 휠 위로
            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                selectedmagic = (selectedmagic + 1) % magic.Length;
            }
            //마우스 휠 아래로 
            if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                selectedmagic--;
                if (selectedmagic < 0)
                {
                    selectedmagic = magic.Length - 1;
                }
            }
            //선택된 마법인 변경될 경우 다른 마법 선택
            if (previousSelectedMagic != selectedmagic)
            {
                Selectmagic();
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
                magic[selectedmagic].GetComponent<TotalWeapon>().Shoot();

            }
            else
            {
                // 무기 사용
                //weapons[selectedWeapon].GetComponent<WeaponBase>().Use();
                weapons[selectedWeapon].GetComponent<TotalWeapon>().Shoot();
            }
        }
    }

    //선택 무기 활성화
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
    //선택 마법 활성화
 void Selectmagic()
    {
        for (int i = 0; i < magic.Length; i++)
        {
            magic[i].SetActive(i == selectedmagic && isMagicActive);
            if (i == selectedmagic && isMagicActive)
            {
                magic[i].transform.SetParent(leftHand);
                magic[i].transform.localPosition = Vector3.zero;
                magic[i].transform.localRotation = Quaternion.identity;
            }
        }
    }
    //무기나 마법 상태에 따라 오른팔과 왼팔 설정
    void SelectWeaponOrMagic()
    {
        if (isMagicActive)
        {
            leftArm.SetActive(true);
            rightArm.SetActive(false);
            leftArm.transform.SetParent(leftHand);
            //magic.transform.localPosition = Vector3.zero;
            //magic.transform.localRotation = Quaternion.identity;

            for (int i = 0; i < weapons.Length; i++)
            {
                weapons[i].SetActive(false);
            }
            Selectmagic();
        }
        else
        {
            leftArm.SetActive(false);
            rightArm.SetActive(true);

            for (int i = 0; i < magic.Length; i++)
            {
                magic[i].SetActive(false);
            }


            SelectWeapon();
        }
    }
}
