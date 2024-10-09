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

    private bool isFlipped;

    private Vector2 destination;

    [SerializeField]
    private SpriteRenderer sign;

    [SerializeField]
    private Sprite idleSprite;
    [SerializeField]
    private Sprite searchSprite;
    [SerializeField]
    private Sprite attackSprite;

    [SerializeField]
    private LayerMask hitLayers;


    public bool Flip
    {
        get => isFlipped;
    }

    void Awake()
    {
        isFlipped = false;
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    public void reactToSound(Vector2 soundPos, float soundLevel)
    {
        if (soundLevel > 0 && soundLevel <= 8)
        {
            state = EnemyState.SEARCH;
            destination = soundPos;
            isStressed = true;
        } 
        else if (soundLevel > 8)
        {
            state = EnemyState.ATTACK;
            isStressed = true;
        }
    }

    private bool isStressed = false;

    private void checkEnemy()
    {
        if (target != null)
        {
            Vector2 direction = (target.transform.position - transform.position).normalized;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 8, hitLayers);
            Debug.DrawRay(transform.position, direction * 8, Color.cyan);

            if (hit)
            {
                if (hit.collider.tag == "Player")
                {
                    state = EnemyState.ATTACK;
                    isStressed = true;
                } else
                {
                    StartCoroutine(swapState());
                }
            }
            else
            {
                StartCoroutine(swapState());
            }
        }
    }

    private IEnumerator swapState()
    {
        yield return new WaitForSeconds(2f);

        if (isStressed) state = EnemyState.SEARCH;
        else state = EnemyState.IDLE;
    }

    void Update()
    {
        switch (state)
        {
            case EnemyState.IDLE:
                checkEnemy();
                sign.sprite = idleSprite;
                break;
            case EnemyState.SEARCH:
                checkEnemy();
                if (agent.isOnNavMesh)
                {
                    agent.SetDestination(destination);
                }
                sign.sprite = searchSprite;
                break;
            case EnemyState.ATTACK:
                if (target != null && agent.isOnNavMesh)
                {
                    agent.SetDestination(target.position);
                    isStressed = true;
                }
                sign.sprite = attackSprite;
                break;
            case EnemyState.DEAD:
                Kill();
                sign.sprite = null;
                break;
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
