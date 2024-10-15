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

    public void setStop()
    {
        Destroy(this);
        stop = true;
    }

    public void swapWeapon()
    {
        ResetSprite();

        currentAmmo = currentWeapon.maxAmmo;
        animator.runtimeAnimatorController = currentWeapon.weaponAnimator;
        audioSource.clip = currentWeapon.weaponSound;
    }

    private void Update()
    {
        if (stop) return;

        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
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

    void DetectEnemiesInSoundRange()
    {
        // 사운드 범위 내에 있는 모든 적 탐지
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

    void LookAtDirection2D(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        //transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        //transform.rotation = Quaternion.Euler(0, 0, mp ? angle += 180f : angle);

        //Vector2 lookingDirection = GetComponentInParent<EnemyManager>().Flip ? -transform.right : transform.right;

        //Debug.DrawLine(transform.position, transform.position + (Vector3)lookingDirection * 5f, Color.red);
    }

    void Shoot()
    {
        if (isReloading) {
            print("reloading!");
            return;
        }
        if (currentAmmo <= 0)
        {
            Reload();
            Debug.Log("Out of ammo, reload!");
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
                    Debug.Log("Hit: " + hit.collider.name);
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
                Debug.Log("Hit: " + hit.collider.name);
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

    private IEnumerator DisableMuzzleFlash()
    {
        yield return new WaitForSeconds(0.2f);
        //muzzle.SetActive(false);
    }

    private bool isReloading = false;


    void Reload()
    {
        if (isReloading) return;

        Debug.Log("Reloading...");

        isReloading = true;

        audioSource.clip = reloadClip;
        audioSource.Play();

        StartCoroutine(addAmmo(currentWeapon.maxAmmo));
    }

    private IEnumerator addAmmo(int ammo)
    {
        yield return new WaitForSeconds(2f);

        isReloading = false;

        currentAmmo = ammo;

        Debug.Log("Reloaded!");

        audioSource.clip = currentWeapon.weaponSound;
    }

    void ResetSprite()
    {
        GetComponent<SpriteRenderer>().sprite = currentWeapon.idleSprite;
    }

    public List<Transform> particles;

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
