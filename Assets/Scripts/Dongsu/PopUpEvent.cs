using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpEvent : MonoBehaviour
{
    [TextArea] public string text;
    public float time;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            UiManager.instance.DialoguePopUp(text, time);
            Destroy(gameObject);
        }
    }
}
