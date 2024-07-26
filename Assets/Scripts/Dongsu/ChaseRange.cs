using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class ChaseRange : MonoBehaviour
{
    EnemyState enemyState;
    GameObject enemy;

    public int chaseRange = 20;

    RaycastHit hitInfo;

    bool chaseCheck = false;

    GameObject target;

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
        if (other.gameObject.CompareTag("Player"))
        {
            target = other.gameObject;

            StartCoroutine(RetryRay());
        }
    }

    private IEnumerator RetryRay()
    {
        while (chaseCheck == false)
        {
            if (Physics.Raycast(enemy.transform.position, target.transform.position, out hitInfo, chaseRange))
            {
                if (hitInfo.transform.gameObject.CompareTag("Player"))
                {
                    enemyState.ChangeState(EnemyState.State.Chase);

                    chaseCheck = true;
                }
                else
                {
                }
            }
            yield return new WaitForSeconds(2f);
        }
    }
}
