using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

public class WeaponManager : MonoBehaviour
{
    public Weapon currentWeapon;   // Reference to the current weapon ScriptableObject
    public LayerMask hitLayers;    // Layers that the ray can hit

    public int currentAmmo;
    private float nextTimeToFire = 0f;

    [SerializeField]
    private GameObject bullet;

    [SerializeField]
    private GameObject muzzle;

    [SerializeField]
    private TMP_Text ammoTxt;

    private Animator animator;

    private AudioSource audioSource;

    [SerializeField]
    private AudioClip reloadClip;

    //시작 시 기본 설정
    private void Start()
    {
        currentAmmo = currentWeapon.maxAmmo;
        muzzle.SetActive(false);    
        ResetSprite();

        animator = GetComponent<Animator>();
        animator.runtimeAnimatorController = currentWeapon.weaponAnimator;
        audioSource = GetComponent<AudioSource>();

        audioSource.clip = currentWeapon.weaponSound;
    }

    public bool stop = false;

    //플레이어 사망 시 비활성화 대신 사용 불가능하게 만듦
    public void setStop()
    {
        Destroy(this);
        stop = true;
    }

    //무기 변경후 호출시 데이터를 알맞게 설정하게 함
    public void swapWeapon()
    {
        ResetSprite();

        StopAllCoroutines();

        currentAmmo = currentWeapon.maxAmmo;
        animator.runtimeAnimatorController = currentWeapon.weaponAnimator;
        audioSource.clip = currentWeapon.weaponSound;

        isReloading = false;
    }
    
    //매 프레임 출력 전 호출됨. 입력에 따른 무기 발사 및 장전 등의 상호작용 업데이트, ui 업데이트
    private void Update()
    {
        if (stop) return;

        if (Input.GetMouseButton(0) && Time.time >= nextTimeToFire)
        {
            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }

        ammoTxt.text = $"Ammo : {currentAmmo} / {currentWeapon.maxAmmo}";



        /*

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        Vector2 direction = (mousePos - transform.position).normalized;

        //LookAtDirection2D(direction);

        */
    }
    
    //가상의 콜라이더를 이용하여 범위 내 적들에게 소리 크기에 따른 신호 전달.
    void DetectEnemiesInSoundRange()
    {
        // ���� ���� ���� �ִ� ��� �� Ž��
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, currentWeapon.gunSoundDistance, LayerMask.GetMask("Enemy"));

        foreach (Collider2D enemyCollider in enemiesInRange)
        {
            EnemyManager enemy = enemyCollider.GetComponent<EnemyManager>();
            if (enemy != null)
            {
                enemy.reactToSound(transform.position, currentWeapon.gunSoundDistance - Vector2.Distance((Vector2) transform.position, (Vector2) enemy.transform.position));
            }
        }
    }

    //deprecated 특정 방향벡터 바라보게 함
    void LookAtDirection2D(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        //transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        //transform.rotation = Quaternion.Euler(0, 0, mp ? angle += 180f : angle);

        //Vector2 lookingDirection = GetComponentInParent<EnemyManager>().Flip ? -transform.right : transform.right;

        //Debug.DrawLine(transform.position, transform.position + (Vector3)lookingDirection * 5f, Color.red);
    }

    //발사. 마우스 방향에 따라 복제 오브젝트 발사 및 레이캐스팅을 통한 데미지 처리
    void Shoot()
    {
        if (isReloading) {
            //print("reloading!");
            return;
        }
        if (currentAmmo <= 0)
        {
            Reload();
            //Debug.Log("Out of ammo, reload!");
            return;
        }

        //muzzle.SetActive(true);

        nextTimeToFire = Time.time + 1f / currentWeapon.fireRate;
        currentAmmo--;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        Vector2 direction = (mousePos - transform.position).normalized;

        if (currentWeapon.name == "Shotgun")
        {
            float baseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            for (int i = 0; i < 12; i++)
            {
                float randomSpread = Random.Range(-45f / 2f, 45f / 2f);

                float finalAngle = baseAngle + randomSpread;
                Vector2 shotDirection = new Vector2(Mathf.Cos(finalAngle * Mathf.Deg2Rad), Mathf.Sin(finalAngle * Mathf.Deg2Rad));

                GameObject obj = Instantiate(bullet, muzzle.transform.position, Quaternion.identity);
                Projectile pj = obj.GetComponent<Projectile>();
                pj.direction = shotDirection;

                RaycastHit2D hit = Physics2D.Raycast(transform.position, shotDirection, Mathf.Infinity, hitLayers);

                Debug.DrawRay(transform.position, shotDirection * 100, Color.red, 1f);

                DetectEnemiesInSoundRange();

                audioSource.Play();
                animator.Play("Fire");

                if (hit)
                {
                    //Debug.Log("Hit: " + hit.collider.name);
                    EnemyManager enemy = hit.collider.GetComponent<EnemyManager>();
                    if (enemy != null)
                    {
                        enemy.gainDamage(currentWeapon.damage);
                    }
                    else
                    {
                        EnemyBoss boss = hit.collider.GetComponent<EnemyBoss>();
                        if (boss != null)
                        {
                            boss.gainDamage(currentWeapon.damage);
                        }
                    }
                }

                //GetComponent<SpriteRenderer>().sprite = currentWeapon.shootSprite;

                Invoke(nameof(ResetSprite), 0.1f);

                //StartCoroutine(DisableMuzzleFlash());
            }
        } 
        else
        {
            GameObject obj = Instantiate(bullet, muzzle.transform.position, Quaternion.identity);
            Projectile pj = obj.GetComponent<Projectile>();
            pj.direction = direction;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, Mathf.Infinity, hitLayers);

            Debug.DrawRay(transform.position, direction * 100, Color.red, 1f);

            DetectEnemiesInSoundRange();

            audioSource.Play();
            animator.Play("Fire");

            if (hit)
            {
                //Debug.Log("Hit: " + hit.collider.name);
                EnemyManager enemy = hit.collider.GetComponent<EnemyManager>();
                if (enemy != null)
                {
                    enemy.gainDamage(currentWeapon.damage);
                }
                else
                {
                    EnemyBoss boss = hit.collider.GetComponent<EnemyBoss>();
                    if (boss != null) {
                        boss.gainDamage(currentWeapon.damage);
                    }
                }
            }

            //GetComponent<SpriteRenderer>().sprite = currentWeapon.shootSprite;

            Invoke(nameof(ResetSprite), 0.1f);

            //StartCoroutine(DisableMuzzleFlash());
        }
    }

    //deprecated
    private IEnumerator DisableMuzzleFlash()
    {
        yield return new WaitForSeconds(0.2f);
        //muzzle.SetActive(false);
    }

    private bool isReloading = false;

    //장전, 코루틴으로 호출
    void Reload()
    {
        if (isReloading) return;

        //Debug.Log("Reloading...");

        isReloading = true;

        audioSource.clip = reloadClip;
        audioSource.Play();

        StartCoroutine(addAmmo(currentWeapon.maxAmmo));
    }

    //장전 코루틴. 2초 후 총알 넣어줌
    private IEnumerator addAmmo(int ammo)
    {
        yield return new WaitForSeconds(2f);

        isReloading = false;

        currentAmmo = ammo;

        //Debug.Log("Reloaded!");

        audioSource.clip = currentWeapon.weaponSound;
    }

    //스프라이트 초기화
    void ResetSprite()
    {
        GetComponent<SpriteRenderer>().sprite = currentWeapon.idleSprite;
    }
    
    public List<Transform> particles;

    //deprecated
    public void flipFirePointPosition()
    {
        foreach (Transform p in particles)
        {
            Vector3 localScale = p.localScale;
            localScale.x *= -1;
            p.localScale = localScale;
        }
    }
}
