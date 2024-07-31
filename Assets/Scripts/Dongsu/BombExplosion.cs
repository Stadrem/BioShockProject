using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombExplosion : MonoBehaviour
{
    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInParent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player") || (other.transform.CompareTag("Enemy")))
        {
            anim.SetTrigger("BombRed");

            StartCoroutine(BombTime());
        }
    }

    IEnumerator BombTime()
    {
        yield return new WaitForSeconds(3.0f);

        Collider[] hits = Physics.OverlapSphere(transform.position, 3.0f);

        foreach(Collider other in hits)
        {
            print(other.transform.name);
            if (other.gameObject.CompareTag("Player"))
            {
                GameManager.instance.Damaged(2);
            }
            else if (other.gameObject.CompareTag("Enemy"))
            {
                Damaged enemy = other.gameObject.GetComponent<Damaged>();
                enemy.Damage(3, "Bomb");
            }
            else if (other.gameObject.CompareTag("Boss"))
            {
                BossDamaged enemy = other.gameObject.GetComponent<BossDamaged>();
                enemy.Damaged(3, "Bomb");
            }
        }
        Destroy(transform.root.gameObject);
    }
}
