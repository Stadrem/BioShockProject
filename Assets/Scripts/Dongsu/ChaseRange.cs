using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class ChaseRange : MonoBehaviour
{
    public EnemyState enemyState;
    public GameObject enemy;

    public int chaseRange = 20;

    RaycastHit hitInfo;

    GameObject target;

    //public LayerMask layerMask;

    bool serching = false;

    // Start is called before the first frame update
    void Start()
    {
        enemy = transform.root.gameObject;
        enemyState = GetComponentInParent<EnemyState>();

        if (enemyState == null)
        {
            Debug.LogError("ParentScript not found in parent objects.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && GameManager.instance.isDie == false)
        {
            print("플레이어입장");

            serching = true;

            target = other.gameObject;

            if(serching == true)
            {
                StartCoroutine(RetryRay(other.gameObject));
            }
        }
    }

    private IEnumerator RetryRay(GameObject tar)
    {
        while (serching == true)
        {
            if (Physics.Raycast(transform.position, tar.transform.position - transform.position, out hitInfo, chaseRange+20, enemyState.layerMask))
            {
                print("Ray 발사  " + hitInfo.collider.name);

                Debug.DrawRay(transform.position, tar.transform.position - transform.position, Color.red, 1.0f);

                if (hitInfo.collider.gameObject == GameManager.instance.player)
                {
                    print("추적 시작");

                    enemyState.ChangeState(EnemyState.State.Chase);

                    serching = false;
                }
            }
            else
            {
                print("아무것도 없는데요?");
            }
            yield return new WaitForSeconds(3.0f);
        }
    }

    /*
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            enemyState.ChangeState(EnemyState.State.Chase);

            serching = false;
        }
    }
    */
}
