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
        print("입장");
        if (other.gameObject.CompareTag("Player"))
        {
            target = other.gameObject;
            print("레이");
            StartCoroutine(RetryRay());
        }
    }

    private IEnumerator RetryRay()
    {
        print("코루틴");
        while (chaseCheck == false)
        {
            print("반복문");
            if (Physics.Raycast(enemy.transform.position, target.transform.position, out hitInfo, chaseRange))
            {
                if (hitInfo.transform.gameObject.CompareTag("Player"))
                {
                    print("인식완료");

                    enemyState.ChangeState(EnemyState.State.Chase);

                    chaseCheck = true;
                }
                else
                {
                    print("인식불가");
                }
            }
            yield return new WaitForSeconds(2f);
        }
    }
}
