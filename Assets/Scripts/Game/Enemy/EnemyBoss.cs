using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class EnemyBoss : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    [SerializeField]
    private int health;
    [SerializeField]
    private int damage;
    [SerializeField]
    private LayerMask hitLayers;

    [SerializeField]
    private AudioClip hitAudioClip;
    [SerializeField]
    private AudioClip dieAudioClip;
    [SerializeField]
    private AudioClip idleAudioClip;

    [SerializeField]
    private UnityEngine.UI.Slider hpBar;

    private AudioSource audioSource;
    NavMeshAgent agent;
    private bool isFlipped;
    public bool isDead = false;

    private float minInterval = 3f;
    private float maxInterval = 12f;

    private Animator animator;

    public UnityEvent onBossClear;

    private void setMaxHealth(int health)
    {
        hpBar.maxValue = health;
        hpBar.value = health;
    }

    private void Awake()
    {
        isFlipped = false;
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        setMaxHealth(health);

        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (target == null) target = GameObject.FindGameObjectWithTag("Player").transform;

        StartCoroutine(playIdleSound());
    }

    private IEnumerator playIdleSound()
    {
        while (!isDead)
        {
            float interval = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(interval);

            //print($"{gameObject.name}");
            audioSource.clip = idleAudioClip;
            audioSource.Play();
        }
    }

    private bool isCastingSkill = false;

    [SerializeField]
    private GameObject attackLightning;

    [SerializeField]
    private float lightningDelay;

    private void SkillLightning()
    {
        for (int dx = 7; dx <= 26; dx++) //0.5씩 더하기
        {
            for (int dy = 15; dy <= 33; dy++)
            {
                Vector2 dPos = new Vector2(dx + 0.5f, dy + 0.5f);

                GameObject l = Instantiate(attackLightning, dPos, Quaternion.identity);

                Destroy(l, 1f);
            }
        }
        target.gameObject.GetComponent<PlayerManager>().gainDamage((int) target.gameObject.GetComponent<PlayerManager>().Health / 10);
    }

    [SerializeField]
    private GameObject attackRock;
    [SerializeField]
    private float rockDelay;

    private void SkillThrowRock()
    {
        Vector2 direction = (target.transform.position- transform.position).normalized;
        GameObject rock = Instantiate(attackRock, transform.position, Quaternion.identity);
        Projectile pj = rock.GetComponent<Projectile>();
        pj.direction = direction;
    }

    float totalRotationDeg = 0f;

    [SerializeField]
    private float rotationSpeed = 30f;
    [SerializeField]
    private float laserDelay;
    [SerializeField]
    private GameObject attackLaser;
    private GameObject las;

    private void SkillLaser()
    {
        las = Instantiate(attackLaser, transform.position, Quaternion.identity);
        isCastingSkill = true;
    }

    void Start()
    {
        updateSKill();
    }

    private void updateSKill()
    {
        StartCoroutine(lightning());
        StartCoroutine(rock());
        StartCoroutine(laser());
    }

    private IEnumerator lightning()
    {
        while (!isDead)
        {
            if (isDead) yield break;
            yield return new WaitForSeconds(lightningDelay);
            SkillLightning();
        }
    }

    private IEnumerator rock()
    {
        while (!isDead)
        {
            if (isDead) yield break;
            yield return new WaitForSeconds(rockDelay);
            SkillThrowRock();
        }
    }

    private IEnumerator laser()
    {
        while (!isDead)
        {
            if (isDead) yield break;
            yield return new WaitForSeconds(laserDelay);
            SkillLaser();
        }
    }

    void Update()
    {
        if (isDead) return;
        if (isCastingSkill)
        {
            if (totalRotationDeg < 360f)
            {
                las.transform.position = transform.position;
                totalRotationDeg += (rotationSpeed * Time.deltaTime);
                las.transform.Rotate(0f, 0f, -rotationSpeed * Time.deltaTime);

                /*
                RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, Mathf.Infinity, hitLayers);

                Debug.DrawRay(transform.position, dir * 100, Color.yellow, 0.5f);

                if (hit)
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        target.gameObject.GetComponent<PlayerManager>().gainDamage(target.gameObject.GetComponent<PlayerManager>().Health / 50);
                    }
                } */
            } else
            {
                totalRotationDeg = 0f;
                isCastingSkill = false;
                Destroy(las);
            }
        }

        if (agent.isOnNavMesh)
        {
            agent.SetDestination(target.position);
        }
    }

    public void Kill()
    {
        FindAnyObjectByType<GameManager>().bossClear();
        FindAnyObjectByType<PlayerManager>().bossClear();

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
        StopAllCoroutines();
        DestroyImmediate(attackLaser, true);
        //DestroyImmediate(attackRock, true);
        Destroy(gameObject, 15f);
    }

    public void gainDamage(int amount)
    {
        health -= amount;
        hpBar.value = health;
        if (health <= 0)
        {
            Kill();
        }
        else
        {
            audioSource.clip = hitAudioClip;
            audioSource.Play();
            animator.Play("Hit");
        }
    }
}
