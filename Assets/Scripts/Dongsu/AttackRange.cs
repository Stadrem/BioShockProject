using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackRange : MonoBehaviour
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
        if (other.gameObject == GameManager.instance.player)
        {
            enemyState.ChangeState(EnemyState.State.Attack);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == GameManager.instance.player)
        {
            enemyState.ChangeState(EnemyState.State.Chase);
        }
    }
}
