using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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

    private bool isCastingSkill = false;

    [SerializeField]
    private GameObject attackLightning;

    [SerializeField]
    private float lightningDelay;

    private void SkillLightning()
    {
        for (int dx = 3; dx <= 22; dx++) //0.5씩 더하기
        {
            for (int dy = -10; dy <= 9; dy++)
            {
                Vector2 dPos = new Vector2(dx + 0.5f, dy + 0.5f);

                Instantiate(attackLightning, dPos, Quaternion.identity);

                target.gameObject.GetComponent<PlayerManager>().gainDamage(target.gameObject.GetComponent<PlayerManager>().Health / 10);
            }
        }
    }

    [SerializeField]
    private GameObject attackRock;
    [SerializeField]
    private float rockDelay;

    private void SkillThrowRock()
    {
        if (isCastingSkill) return;
        Vector2 direction = (target.transform.position - transform.position).normalized;

        GameObject rock = Instantiate(attackRock, transform.position, Quaternion.identity);
        Projectile pj = rock.GetComponent<Projectile>();

        pj.direction = direction;
    }

    Vector2 dir;
    float totalRotationDeg = 0f;

    [SerializeField]
    private float rotationSpeed = 30f;
    [SerializeField]
    private float laserDelay;

    private void SkillLaser()
    {
        dir = (target.transform.position - transform.position).normalized;
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
        while (true)
        {
            yield return new WaitForSeconds(lightningDelay);
            SkillLightning();
        }
    }

    private IEnumerator rock()
    {
        while (true)
        {
            yield return new WaitForSeconds(rockDelay);
            SkillThrowRock();
        }
    }

    private IEnumerator laser()
    {
        while (true)
        {
            yield return new WaitForSeconds(laserDelay);
            SkillLaser();
        }
    }

    void Update()
    {
        if (isCastingSkill)
        {
            if (totalRotationDeg < 360f)
            {
                totalRotationDeg += (rotationSpeed * Time.deltaTime);
                transform.Rotate(0f, 0f, -rotationSpeed * Time.deltaTime);
                RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, Mathf.Infinity, hitLayers);

                Debug.DrawRay(transform.position, dir * 100, Color.yellow, 0.5f);

                if (hit)
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        target.gameObject.GetComponent<PlayerManager>().gainDamage(target.gameObject.GetComponent<PlayerManager>().Health / 50);
                    }
                }
            } else
            {
                isCastingSkill = false;
            }
        }
    }
}
