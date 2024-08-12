using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BombExplosion : MonoBehaviour
{
    Animator anim;
    public GameObject bombEffect;
    public LayerMask layerMask;
    bool bombStart = false;
    bool knockBack = false;
    float currentTime = 0;
    float knockTime = 0.3f;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInParent<Animator>();
    }

    private void Update()
    {
        if(knockBack == true)
        {
            currentTime += Time.deltaTime;

            Vector3 knockbackDirection = -GameManager.instance.player.transform.forward * 25 * Time.deltaTime;

            GameManager.instance.player.GetComponent<CharacterController>().Move(knockbackDirection);

            if (currentTime > knockTime)
            {
                knockBack = false;
                currentTime = 0;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(bombStart == false)
        {
            bombStart = true;

            anim.SetTrigger("BombRed");

            StartCoroutine(BombTime());
        }
    }

    IEnumerator BombTime()
    {
        yield return new WaitForSeconds(4.0f);

        Collider[] hits = Physics.OverlapSphere(transform.position, 4.0f, layerMask);

        bombEffect.SetActive(true);

        foreach (Collider other in hits)
        {
            print(other.transform.name);
            if (other.gameObject.CompareTag("Player"))
            {
                GameManager.instance.Damaged(3);
                knockBack = true;
            }
            else if (other.gameObject.CompareTag("Enemy"))
            {
                Damaged enemy = other.gameObject.GetComponent<Damaged>();
                enemy.Damage(7, "Bomb");
            }
            else if (other.gameObject.CompareTag("Boss"))
            {
                BossDamaged enemy = other.gameObject.GetComponent<BossDamaged>();
                enemy.Damaged(7, "Bomb");
            }
        }
        yield return new WaitForSeconds(0.5f);
        Destroy(transform.root.gameObject);
    }
}
