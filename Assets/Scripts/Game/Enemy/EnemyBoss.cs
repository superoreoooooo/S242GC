using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class EnemyBoss : MonoBehaviour
{
    /*
    *
    *  보스에 사용되는 스크립트
    *
    */

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

    //체력바 설정
    private void setMaxHealth(int health)
    {
        hpBar.maxValue = health;
        hpBar.value = health;
    }

    //인스턴스화 직후 호출. 변수 초기화 및 설정
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

    //일정 주기마다 좀비 소리 계속 나게 하는 코루틴 구현부
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
    
    //번개 스킬. 좌표가 하드코딩 되어 있음
    private void SkillLightning()
    {
        for (int dx = 7; dx <= 26; dx++) //0.5�� ���ϱ�
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

    //투석 스킬. 플레이어쪽으로 날림
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

    //레이저 스킬
    private void SkillLaser()
    {
        las = Instantiate(attackLaser, transform.position, Quaternion.identity);
        isCastingSkill = true;
    }

    //시작 시 호출
    void Start()
    {
        updateSKill();
    }

    //스킬 코루틴 실행
    private void updateSKill()
    {
        StartCoroutine(lightning());
        StartCoroutine(rock());
        StartCoroutine(laser());
    }

    //번개 스킬 코루틴. 죽을때까지 일정 주기마다 스킬 캐스팅
    private IEnumerator lightning()
    {
        while (!isDead)
        {
            if (isDead) yield break;
            yield return new WaitForSeconds(lightningDelay);
            SkillLightning();
        }
    }
    
    //투석 스킬 코루틴. 죽을때까지 일정 주기마다 스킬 캐스팅
    private IEnumerator rock()
    {
        while (!isDead)
        {
            if (isDead) yield break;
            yield return new WaitForSeconds(rockDelay);
            SkillThrowRock();
        }
    }

    //레이저 스킬 코루틴. 죽을때까지 일정 주기마다 스킬 캐스팅
    private IEnumerator laser()
    {
        while (!isDead)
        {
            if (isDead) yield break;
            yield return new WaitForSeconds(laserDelay);
            SkillLaser();
        }
    }

    /**
    * 매 프레임 호출. 
    * 1. 레이저 스킬 사용 중 회전
    * 2. NavMesh상에 있을 때 목표 설정
    */
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

    //보스 사망 함수.
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

    //데미지 처리 및 체력바 동기화 (피해 입는거)
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
