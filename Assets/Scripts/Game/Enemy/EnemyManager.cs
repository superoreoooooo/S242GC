using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Experimental.AI;

public class EnemyManager : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    [SerializeField]
    private int health;
    [SerializeField]
    private int damage;
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
    [SerializeField]
    private float detectionRadius;
    [SerializeField]
    private float detectionAngle;

    NavMeshAgent agent;
    public EnemyState state;
    private bool isFlipped;
    private Vector2 lastSeenPos;
    private bool isStressed = false;
    private EnemyState stateBefore;
    private float lastStateChangedTime;
    private Vector3 originPos;
    public Vector2 viewDirection;

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

        originPos = transform.position;

        viewDirection = Vector2.right;
    }

    public Transform fovLight;

    void Update()
    {
        /*
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
                    agent.SetDestination(lastSeenPos);
                }
                sign.sprite = searchSprite;
                break;
            case EnemyState.ATTACK:
                checkEnemy();
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
        }*/

        if (agent.isOnNavMesh)
        {
            stateManager();
            switch (checkEnemy())
            {
                case EnemyState.IDLE:
                    if (Vector2.Distance(originPos, transform.position) >= 0.2f)
                    {
                        agent.SetDestination(originPos);
                    }
                    sign.sprite = idleSprite;
                    break;
                case EnemyState.SEARCH:
                    LookAt(lastSeenPos);
                    agent.SetDestination(lastSeenPos);
                    sign.sprite = searchSprite;
                    break;
                case EnemyState.ATTACK:
                    isStressed = true;
                    if (target != null)
                    {
                        lastSeenPos = target.position;
                        agent.SetDestination(target.position);
                    }
                    sign.sprite = attackSprite;
                    break;
                case EnemyState.DEAD:
                    Kill();
                    sign.sprite = null;
                    break;
                case EnemyState.VOID:
                    break;
            }
        }

        float angle = Mathf.Atan2(viewDirection.y, viewDirection.x) * Mathf.Rad2Deg;
        fovLight.rotation = Quaternion.Euler(0, 0, angle - 90);
    }

    public void reactToSound(Vector2 soundPos, float soundLevel)
    {
        if (soundLevel > 0 && soundLevel <= 8)
        {
            if (state != EnemyState.ATTACK) state = EnemyState.SEARCH;
            lastSeenPos = soundPos;
        }
        else if (soundLevel > 8)
        {
            state = EnemyState.ATTACK;
            isStressed = true;
        }
    }

    void LookAt(Vector2 targetPosition)
    {
        viewDirection = (targetPosition - (Vector2) transform.position).normalized;
    }

    private EnemyState checkEnemy()
    {
        //Legacy code v1
        /*
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
                    lastSeenPos = target.transform.position;
                }
                else
                {
                    if (stateMgr != null) StopCoroutine(stateMgr);

                    stateMgr = swapState(2f);
                    StartCoroutine(stateMgr);
                }
            }
            else
            {
                if (state == EnemyState.ATTACK) {
                    if (stateMgr != null) StopCoroutine(stateMgr);

                    stateMgr = swapState(5f);
                    StartCoroutine(stateMgr);
                }
                else {
                    if (stateMgr != null) StopCoroutine(stateMgr);

                    stateMgr = swapState(2f);
                    StartCoroutine(stateMgr);
                }
            }
        }*/
        if (target != null)
        {
            //Legacy code v2
            Vector2 direction = (target.transform.position - transform.position).normalized;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 8, hitLayers);
            Debug.DrawRay(transform.position, viewDirection * 8, Color.green);
            Debug.DrawRay(transform.position, direction * 8, state == EnemyState.ATTACK ? Color.red : Color.cyan);

            if (hit && hit.collider.CompareTag("Player") && Vector2.Angle(viewDirection, (target.transform.position - transform.position).normalized) <= detectionAngle / 2)
            {
                LookAt(target.transform.position);
                state = EnemyState.ATTACK;
                return EnemyState.ATTACK;
            }
            else
            {
                if (Time.time - lastStateChangedTime >= 3f) //어그로풀림?
                {
                    if (state == EnemyState.ATTACK)
                    {
                        state = EnemyState.SEARCH;
                        return EnemyState.SEARCH;
                    }
                    else if (state == EnemyState.SEARCH)
                    {
                        if (isStressed)
                        {
                            state = EnemyState.SEARCH;
                            return EnemyState.SEARCH;
                        }
                        else
                        {
                            state = EnemyState.IDLE;
                            return EnemyState.IDLE;
                        }
                    }
                    else
                    {
                        return state;
                    }
                }
                else
                {
                    return state;
                }
            }
            //Legacy code v3
            /*
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius, hitLayers);
            foreach (Collider2D collider in hitColliders)
            {
                Vector2 targetDirection = (collider.transform.position - transform.position).normalized;

                float angleBetween = Vector2.Angle(viewDirection, targetDirection);

                if (angleBetween <= detectionAngle / 2)
                {
                    Debug.Log("Detected target within cone: " + collider.name);
                    if (collider.CompareTag("Player"))
                    {
                        Vector2 direction = (target.transform.position - transform.position).normalized;
                        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 8, hitLayers);
                        Debug.DrawRay(transform.position, direction * 8, state == EnemyState.ATTACK ? Color.red : Color.cyan);

                        if (hit && hit.collider.CompareTag("Player"))
                        {
                            state = EnemyState.ATTACK;
                            return EnemyState.ATTACK;
                        }
                        else
                        {
                            if (Time.time - lastStateChangedTime >= 3f) //어그로풀림?
                            {
                                if (state == EnemyState.ATTACK)
                                {
                                    state = EnemyState.SEARCH;
                                    return EnemyState.SEARCH;
                                }
                                else if (state == EnemyState.SEARCH)
                                {
                                    if (isStressed)
                                    {
                                        state = EnemyState.SEARCH;
                                        return EnemyState.SEARCH;
                                    }
                                    else
                                    {
                                        state = EnemyState.IDLE;
                                        return EnemyState.IDLE;
                                    }
                                }
                                else
                                {
                                    return state;
                                }
                            }
                            else
                            {
                                return state;
                            }
                        }
                    }
                    else return EnemyState.VOID;
                }
                else return EnemyState.VOID;
            }
            return EnemyState.VOID;
            */
        }
        else return EnemyState.VOID;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    private void stateManager()
    {
        if (stateBefore != state)
        {
            lastStateChangedTime = Time.time;
            print($"TT : {lastStateChangedTime}");
            stateBefore = state;
        }
    }

    private IEnumerator swapState(float duration)
    {
        yield return new WaitForSeconds(duration);

        if (isStressed) state = EnemyState.SEARCH;
        else state = EnemyState.IDLE;
    }

    private void flip()
    {
        isFlipped = !isFlipped;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
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
