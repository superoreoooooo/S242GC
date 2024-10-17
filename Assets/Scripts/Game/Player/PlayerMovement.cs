using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public bool isMoveable = true;

    [SerializeField]
    private float speed = 5f;

    [SerializeField]
    private float runSpeed = 8f;

    private Vector2 movement;

    public Vector2 Movement
    {
        get { return movement; }
        set { movement = value; }
    }

    private Rigidbody2D rb;

    public bool isRunning;

    private bool isDashing;

    public bool IsDashing {
        get => isDashing;
    }

    [SerializeField]
    private int dashDamage;

    public int DashDamage {
        get => dashDamage;
    }

    [SerializeField]
    private float dashForce;

    [SerializeField]
    private float dashDuration;

    [SerializeField]
    private float dashThickness;

    private bool isDashAble = true;
    
    private float coolDownDash = 0f;
    
    [SerializeField]
    private float CoolDown_DASH;

    private float originalSpeed;

    [SerializeField]    
    private WeaponManager weaponManager;

    [SerializeField]
    private float walkSoundLevel;

    [SerializeField]
    private float runSoundLevel;
    
    //변수 초기화 및 설정
    void Start()
    {
        isFlipped = false;
        originalSpeed = speed;
        isDashing = false;
        movement = new Vector2();
        rb = GetComponent<Rigidbody2D>();

        isRunning = false;

        try { 
            weaponManager = GetComponentInChildren<WeaponManager>(); 
        } finally {
            print("weaponManager not loaded!");
            weaponManager = null; 
        }
        animator = GetComponent<Animator>();
        playerManager = GetComponent<PlayerManager>();
    }

    //사망 처리
    public void die()
    {
        if (playerManager.isDead)
        {
            movement = Vector2.zero;
            rb.velocity = Vector2.zero;
        }
    }

    private Animator animator;

    //매 프레임 업데이트. 입력 받아 이동 관련 요소 및 스킬 사용 업데이트
    void Update()
    {
        if (playerManager.isDead)
        {
            return;
        }
        if (isMoveable)
        {
            if (isDashing) return;
            if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
            {
                movement.Set(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
                DetectEnemiesInSoundRange();
            }
            if (Input.GetKey(KeyCode.LeftControl) && isDashAble)
            {
                StartCoroutine(SkillDash());
            }
            if (Input.GetKey(KeyCode.LeftShift))
            {
                isRunning = true;
            } else
            {
                isRunning = false;
            }
        }

        if (isRunning)
        {
            animator.SetBool("isRunning", true);
        } else
        {
            animator.SetBool("isRunning", false);
        }

        UpdateSkill();

        if (movement.x < 0 && !isFlipped)
        {
            flip();
        }
        else if (movement.x > 0 && isFlipped)
        {
            flip();
        } 
    }

    //가상의 콜라이더를 이용하여 범위 내 적들에게 소리 크기에 따른 신호 전달. (발소리)
    void DetectEnemiesInSoundRange()
    {
        // ���� ���� ���� �ִ� ��� �� Ž��
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, isRunning ? runSoundLevel : walkSoundLevel, LayerMask.GetMask("Enemy"));

        foreach (Collider2D enemyCollider in enemiesInRange)
        {
            EnemyManager enemy = enemyCollider.GetComponent<EnemyManager>();
            if (enemy != null)
            {
                enemy.reactToSound(transform.position, isRunning ? runSoundLevel : walkSoundLevel - Vector2.Distance((Vector2)transform.position, (Vector2)enemy.transform.position));
            }
        }
    }

    private PlayerManager playerManager;

    //FixedUpdate. 속력 설정
    private void FixedUpdate() {
        if (playerManager.isDead) return;
        if (isRunning) rb.velocity = movement * runSpeed;
        else rb.velocity = movement * speed;
    }

    //스킬 쿨타임 업데이트
    private void UpdateSkill() {
        updateDashCoolDown();
    }

    //대쉬 (구르기) 쿨타임 업데이트
    private void updateDashCoolDown() {
        if (!isDashAble) {
            coolDownDash += Time.deltaTime;
            if (coolDownDash > CoolDown_DASH) {
                isDashAble = true;
                coolDownDash = 0f;
            }
        }
    }

    //대쉬 (구르기) 코루틴 구현부
    private IEnumerator SkillDash() {
        if (movement.magnitude != 0) {
            isDashing = true;
            Vector2 startPoint = transform.position;
            speed = dashForce;
            Dash(movement.normalized);
            GetComponent<PlayerManager>().Invincible = true;

            yield return new WaitForSeconds(dashDuration);

            speed = originalSpeed;
            Dash(movement.normalized);
            isDashAble = false;

            Vector2 endPoint = transform.position;

            Vector2 dir = endPoint - startPoint;
            float dt = dir.magnitude;
            /*

            Collider2D[] hitColliders = Physics2D.OverlapCapsuleAll(dir / 2,
                new Vector2(dt, dashThickness),
                CapsuleDirection2D.Horizontal,
                0f); //, LayerMask.NameToLayer("Enemy"));

            foreach (Collider2D collider in hitColliders)
            {
                if (collider.tag == "Enemy")
                {
                    collider.gameObject.GetComponent<EnemyManager>().gainDamage(5);
                }
                //print(collider.name);
            } */ 

            GetComponent<PlayerManager>().Invincible = false;

            yield return new WaitForSeconds(0.2f);

            isDashing = false;
        }
    }

    //대쉬 (구르기)
    public void Dash(Vector2 dir) {
        movement = new Vector2(dir.x * speed, dir.y * speed);
    }

    private bool isFlipped;

    public bool Flip
    {
        get => isFlipped;
    }

    //플레이어 양옆 회전. Localscale 조절하여 구현함
    private void flip()
    {
        isFlipped = !isFlipped;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;



        if (weaponManager != null)
        {
            weaponManager.flipFirePointPosition();
        }
    }
}
