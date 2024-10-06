using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Experimental.AI;

public class EnemyManager : MonoBehaviour
{
    [SerializeField]
    private Transform target;

    NavMeshAgent agent;

    [SerializeField]
    private int health;

    [SerializeField]
    private int damage;

    public EnemyState state;

    private Vector2 headDir;

    private bool isFlipped;

    public bool Flip
    {
        get => isFlipped;
    }

    public Vector2 HeadDir
    {
        get => headDir;
    }

    void Awake()
    {
        isFlipped = false;
        headDir = new Vector2();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    void Update()
    {
        switch (state)
        {
            case EnemyState.IDLE:
                headDir = target.transform.position - transform.position;
                //행동 구현
                break;
            case EnemyState.SEARCH:
                break;
            case EnemyState.ATTACK:
                if (target != null && agent.isOnNavMesh)
                {
                    agent.SetDestination(target.position);
                }
                break;
        }

        if (headDir.x < 0 && !isFlipped)
        {
            flip();
        }
        else if (headDir.x > 0 && isFlipped)
        {
            flip();
        }
    }

    private void flip()
    {
        isFlipped = !isFlipped;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    private void updateState()
    {
        state = EnemyState.ATTACK;
    }

    public void Kill()
    {
        Destroy(gameObject);
    }

    public void gainDamage(int amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Kill();
        }
    }
}
