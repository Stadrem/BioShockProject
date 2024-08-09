using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeviceEnter : MonoBehaviour
{
    public GameObject device;

    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            GameManager.instance.resurrectCap = device;
        }
    }
}
