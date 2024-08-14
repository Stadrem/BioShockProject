using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ObjectBomb : MonoBehaviour
{

    public GameObject effectPrefab;
    public GameObject fireEffect;
    public LayerMask layerMask;
    public LayerMask throwdamMask;
    bool trigger = false;
    public float range = 10f;

    
    void OnCollisionEnter(Collision other)
    {
        if(trigger == false)
        {

            GameObject bulletImpact = Instantiate(effectPrefab);

            Collider[] hitColliders = Physics.OverlapSphere(bulletImpact.transform.position, range);

            foreach (Collider collider in hitColliders)
            {
                // Enemy인지 확인하고 Damage 함수를 호출
                Damaged dam = collider.gameObject.GetComponent<Damaged>();
                if (dam != null)
                {
                    dam.Damage(5, "type");
                }

                // Boss인지 확인하고 Damaged 함수를 호출
                BossDamaged Bodam = collider.gameObject.GetComponent<BossDamaged>();
                if (Bodam != null)
                {
                    Bodam.Damaged(5, "type");
                }
            }

            // bulletImpact 위치 초기화
            bulletImpact.transform.position = transform.position;

            trigger = true;

            Destroy(gameObject, 0.25f);
        }
        
    }
}
