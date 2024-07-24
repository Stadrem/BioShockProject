using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyState : MonoBehaviour
{
    public float speed = 25.0f;

    Rigidbody rb;

    Vector3 dir;

    bool ChaseOn = false;

    public enum State
    {
        Idle,
        Patroll,
        Chase,
        Attack,
        Stun,
        Damaged,
        Die
    }

    public GameObject AttackRange;
    public GameObject ChaseRange;

    public State currentState;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case State.Idle:
                break;

            case State.Patroll:
                break;

            case State.Chase:
                transform.LookAt(GameManager.instance.player.transform);
                ChaseOn = true;
                break;

            case State.Attack:
                break;

            case State.Stun:
                break;

            case State.Damaged:
                break;

            case State.Die:
                break;
        }

        if (ChaseOn == true)
        {
            ChaseState();
        }
    }
    
    public void ChangeState(State newState)
    {
        currentState = newState;
    }

    void ChaseState()
    {
        dir = GameManager.instance.player.transform.position - transform.position;

        dir.Normalize();

        //transform.position += dir * speed * Time.deltaTime;

        rb.velocity = dir * speed;
    }
}
