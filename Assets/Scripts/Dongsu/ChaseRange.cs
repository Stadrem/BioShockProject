using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseRange : MonoBehaviour
{
    public EnemyState enemyState;
    GameObject enemy;

    public int chaseRange = 20;

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
        RaycastHit hitInfo;
        print("입장");
        if (Physics.Raycast(enemy.transform.position, other.transform.position, out hitInfo, chaseRange))
        {
            print("레이발사");
            if (hitInfo.transform.gameObject.CompareTag("Player"))
            {
                print("인식완료");
                enemyState.ChangeState(EnemyState.State.Chase);
            }
            else
            {
                print("인식불가");
            }
        }
    }
    /*
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == GameManager.instance.player)
        {
            enemyState.ChangeState(EnemyState.State.Idle);
        }
    }
    */
}
