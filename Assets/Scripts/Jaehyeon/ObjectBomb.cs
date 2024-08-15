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
    public ParticleSystem fuseFireEffect; // 심지에 붙일 파티클 시스템



    void Start()
    {
        // 만약 Particle System이 폭탄 오브젝트의 자식으로 존재하지 않는다면, 
        // Hierarchy에서 찾아서 할당합니다.
        if (fuseFireEffect == null)
        {
            fuseFireEffect = GetComponentInChildren<ParticleSystem>();
        }
    }


    public void ActivateFuse()
    {
        if (fuseFireEffect != null)
        {
            fuseFireEffect.Play();
        }
        else
        {
            Debug.LogError("Fuse fire effect not found!");
        }
    }


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
