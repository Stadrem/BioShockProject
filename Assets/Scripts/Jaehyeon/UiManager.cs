using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager instance;

    public int bulletCurrent; // 현재 장전된 총알 수
    public int[] keepItems; // 보유중인 총알의 갯수 배열, [2]에 총알 갯수 설정

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 총알 발사 시 호출
    public bool BulletShoot()
    {
        if (bulletCurrent > 0)
        {
            bulletCurrent--;
            return true;
        }
        else
        {
            bulletCurrent = 0;
            return false;
        }
    }

    // 재장전 시 호출
    public void Reload(int magazineSize)
    {
        //현재 가지고 있는 총알이 총 들어가는 총알보다 크다면 총 들어가는 총알으로
        //아니라면 현재 총알양만큼 투입
        int bulletsToLoad = keepItems[0] > magazineSize ? magazineSize : keepItems[0];
        //Mathf.Min(keepItems[2], magazineSize - bulletCurrent);

        keepItems[0] -= bulletsToLoad;
        bulletCurrent += bulletsToLoad;
    }
}
