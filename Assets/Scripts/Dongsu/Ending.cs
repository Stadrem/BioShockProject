using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Ending : MonoBehaviour
{
    public GameObject handToggle;
    public GameObject obj;
    public GameObject effect;
    public GameObject effectPosition;
    public GameObject ui;
    public Image blackOut;
    public CinemachineDollyCart cart;
    float alpha = 0;
    public CinemachineVirtualCamera dolly;
    Color color;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) 
        {
            UiManager.instance.Alret("디버깅 모드 엔딩 활성화");
            EndingPack(); 
        }
    }

    private void Start()
    {
        color = blackOut.color;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //엔딩 이벤트 시작
            EndingPack();
        }
    }

    void EndingPack()
    {
        dolly.gameObject.SetActive(true);

        handToggle.SetActive(false);

        dolly.Priority = 10;

        StartCoroutine(BlackAnim());

        StartCoroutine(ObjDown());

        CartGo();

        EffectShow();

        UiOff();

        SoundManager.instance.EndWaterSound();
    }

    IEnumerator BlackAnim()
    {
        while (true)
        {
            color.a = alpha;
            blackOut.color = color;
            alpha += 0.01f;
            yield return new WaitForSeconds(0.2f);
            if (alpha >= 1) break;
        }
        SceneManager.LoadScene("Ending");
    }

    void CartGo()
    {
        cart.m_Speed = 2.0f;
    }

    void CameraOn()
    {
    }

    IEnumerator ObjDown()
    {
        while (true)
        {
            obj.transform.position -= new Vector3(0, 0.01f, 0);
            yield return new WaitForSeconds(0.05f);
        }
    }

    IEnumerator NextScene()
    {

        yield return new WaitForSeconds(7f);

    }

    void EffectShow()
    {
        GameObject effects = Instantiate(effect);
        effects.transform.position = effectPosition.transform.position;
    }

    void UiOff()
    {
        ui.SetActive(false);
    }
}
