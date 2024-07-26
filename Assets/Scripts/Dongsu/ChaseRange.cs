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

    GameObject target;

    public LayerMask layerMask;

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
            print("플레이어인식");

            target = other.gameObject;

            StartCoroutine(RetryRay());
        }
    }

    private IEnumerator RetryRay()
    {
        while (true)
        {
            if (Physics.Raycast(enemy.transform.position, target.transform.position, out hitInfo, chaseRange, layerMask))
            {
                print("Ray 발사");
                if (hitInfo.transform.gameObject.CompareTag("Player"))
                {
                    enemyState.ChangeState(EnemyState.State.Chase);
                }
            }
            yield return new WaitForSeconds(3.0f);
        }
    }
}
