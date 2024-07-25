using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseRange : MonoBehaviour
{
    public EnemyState enemyState;

    // Start is called before the first frame update
    void Start()
    {
        enemyState = GetComponentInParent<EnemyState>();

        if (enemyState == null)
        {
            Debug.LogError("ParentScript not found in parent objects.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == GameManager.instance.player)
        {
            enemyState.ChangeState("Chase");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == GameManager.instance.player)
        {
            enemyState.ChangeState("Idle");
        }
    }
}
