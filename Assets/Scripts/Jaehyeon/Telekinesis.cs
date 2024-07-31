﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Telekinesis : MonoBehaviour
{
    public LayerMask layerMask; //select
    public LayerMask throwdamMask; //enemy
    public float telekinesisRange = 100f;
    public float telekinesisForce = 10f;
    public float telekinesisGrabbi = 2f;
    private bool grab = false;
    private Transform grabbedObject;
    public GameObject effectPrefab;
    public int damage = 5; // 공격 데미지
    public string type = "?";

    private void Start()
    {
        
    }

    void Update()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            //만약 오브젝트를 당긴다면
            if(!grab)
            {
                SucGrabObject();
            }
            else
            {
                SucThrowObject();
                grab = false;
            }
           
        }

        if (grab && grabbedObject != null)
        {
            // 오브젝트를 플레이어 앞에 계속해서 유지
            grabbedObject.position = Vector3.Lerp(grabbedObject.position, Camera.main.transform.position + Camera.main.transform.forward * telekinesisGrabbi, Time.deltaTime * 10);
            //grabbedObject.position = Camera.main.transform.position + Camera.main.transform.forward * telekinesisGrabbi;
        }
    }

    void SucGrabObject()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, telekinesisRange, layerMask))
        {
            if(hit.collider != null)
            {
                grabbedObject = hit.transform;

                Rigidbody rb = grabbedObject.GetComponent<Rigidbody>();
                if(rb)
                {
                    rb.useGravity = false;
                }

                grab = true;
            }
        }

    }

    void SucThrowObject()
    {
        Rigidbody rb = grabbedObject.GetComponent<Rigidbody>();
        if(rb == null)
        {
            rb =  grabbedObject.gameObject.AddComponent<Rigidbody>();  
        }

        rb.useGravity = true;
        rb.velocity = transform.forward * telekinesisForce;

        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, telekinesisRange, throwdamMask)) //throwdamMask || 1 << LayerMask.NameToLayer("Enemy")
        {
            Debug.Log(hitInfo.transform.name);
            if (hitInfo.collider.CompareTag("Enemy"))
            {
                Damaged damaged = hitInfo.collider.GetComponent<Damaged>();
                damaged.Damage(damage, type);
            }
            else if (hitInfo.collider.CompareTag("Boss"))
            {
                //BossDamaged bossDamaged = hitInfo.collider.GetComponent<BossDamaged>();
                //bossDamaged.BossDamage(1);
            }

           
        }
    }

}