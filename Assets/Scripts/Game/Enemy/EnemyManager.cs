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

    [SerializeField]
    private AudioClip hitAudioClip;
    [SerializeField]
    private AudioClip dieAudioClip;
    [SerializeField]
    private AudioClip idleAudioClip;

    private AudioSource audioSource;
    NavMeshAgent agent;
    public EnemyState state;
    private bool isFlipped;
    private Vector2 lastSeenPos;
    private bool isStressed = false;
    private EnemyState stateBefore;
    private float lastStateChangedTime;
    private Vector3 originPos;
    public Vector2 viewDirection;
    private Vector2 originDir;
    public bool isDead = false;

    private float minInterval = 3f;
    private float maxInterval = 12f;

    [SerializeField]
    private CellDirection viewingDir;

    private Animator animator;

    public bool Flip
    {
        get => isFlipped;
    }

    [SerializeField]
    private UnityEngine.UI.Slider hpBar;

    private void setMaxHealth(int health)
    {
        hpBar.maxValue = health;
        hpBar.value = health;
    }

    void Awake()
    {
        isFlipped = false;
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        setMaxHealth(health);

        originPos = transform.position;

        if (viewingDir == CellDirection.UP) originDir = Vector2.up;
        else if (viewingDir == CellDirection.DOWN) originDir = Vector2.down;
        else if (viewingDir == CellDirection.LEFT) originDir = Vector2.left;
        else if (viewingDir == CellDirection.RIGHT) originDir = Vector2.right;
        else originDir = Vector2.right;

        //originDir = Vector2.right;

        animator = GetComponent<Animator>();

        viewDirection = originDir; //for test
        audioSource = GetComponent<AudioSource>();

        StartCoroutine(playIdleSound());
    }

    private IEnumerator playIdleSound()
    {
        while (!isDead)
        {
            float interval = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(interval);

            print($"{gameObject.name}");
            audioSource.clip = idleAudioClip;
            audioSource.Play();
        }
    }

    public Transform fovLight;
    private Vector2 lastViewPos;

    void Update()
    {
        if (isDead)
        {
            return;
        }
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
                    animator.SetBool("isRunning", false);
                    viewDirection = originDir;
                    if (Vector2.Distance(originPos, transform.position) >= 0.2f)
                    {
                        agent.SetDestination(originPos);
                    }
                    sign.sprite = idleSprite;
                    break;
                case EnemyState.SEARCH:
                    animator.SetBool("isRunning", true);
                    if (Vector2.Distance(lastSeenPos, transform.position) >= 0.2f) {
                        LookAt(lastSeenPos);
                    }
                    agent.SetDestination(lastSeenPos);
                    sign.sprite = searchSprite;
                    break;
                case EnemyState.ATTACK:
                    animator.SetBool("isRunning", true);
                    LookAt(target.transform.position);
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
        if (!agent.isOnNavMesh) return;
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
        viewDirection = (targetPosition - (Vector2)transform.position).normalized;
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

            if (hit && hit.collider.CompareTag("Player") && Vector2.Angle(viewDirection, direction) <= (detectionAngle / 2))
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
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        audioSource.clip = dieAudioClip;
        audioSource.Play();
        animator.Play("Die");
        if (agent.isOnNavMesh) agent.isStopped = true;
        isDead = true;
        gameObject.layer = LayerMask.NameToLayer("DeadEnemy");
        Destroy(gameObject, 15f);
    }

    public void gainDamage(int amount)
    {
        health -= amount;
        hpBar.value = health;
        if (health <= 0)
        {
            Kill();
        } else
        {
            audioSource.clip = hitAudioClip;
            audioSource.Play();
            animator.Play("Hit");
        }
    }
}
