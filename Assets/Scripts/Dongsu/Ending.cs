using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Ending : MonoBehaviour
{
    public GameObject player;
    public GameObject obj;
    public Image blackOut;
    public CinemachineDollyCart cart;
    float alpha = 0;

    public CinemachineVirtualCamera main;
    public CinemachineVirtualCamera dolly;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V)) { EndingPack(); }
    }

    private void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        //엔딩 이벤트 시작
    }

    void EndingPack()
    {
        player.SetActive(false);
        dolly.Priority = 10;
        StartCoroutine(BlackAnim());
        StartCoroutine(ObjDown());
        CartGo();
    }

    IEnumerator BlackAnim()
    {
        while (true)
        {
            blackOut.color = blackOut.color.WithAlpha(alpha);
            alpha += 0.01f;
            yield return new WaitForSeconds(0.1f);
            if (alpha >= 1) break;
        }
    }

    void CartGo()
    {
        cart.m_Speed = 2;
    }

    void CameraOn()
    {
    }

    IEnumerator ObjDown()
    {
        while (true)
        {
            obj.transform.position -= new Vector3(0, 0.05f, 0);
            yield return new WaitForSeconds(0.05f);
        }
    }
}
