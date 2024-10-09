using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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

    private void Start()
    {
        currentAmmo = currentWeapon.maxAmmo;
        muzzle.SetActive(false);
    }

    private void Update()
    {
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

        GameObject obj = Instantiate(bullet, muzzle.transform.position, Quaternion.identity);
        Projectile pj = obj.GetComponent<Projectile>();
        pj.direction = direction;

        // Raycast for shooting
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, Mathf.Infinity, hitLayers);

        // Debug Ray to visualize the shooting
        Debug.DrawRay(transform.position, direction * 100, Color.red, 1f);


        if (hit)
        {
            Debug.Log("Hit: " + hit.collider.name);
            // Apply damage to the hit object if applicable
            EnemyManager enemy = hit.collider.GetComponent<EnemyManager>();
            if (enemy != null)
            {
                enemy.gainDamage(currentWeapon.damage);
            }
        }

        // Change sprite to shoot sprite for feedback (Optional)
        GetComponent<SpriteRenderer>().sprite = currentWeapon.shootSprite;

        // Reset to idle sprite after shooting
        Invoke(nameof(ResetSprite), 0.1f);

        //StartCoroutine(DisableMuzzleFlash());
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

        StartCoroutine(addAmmo(currentWeapon.maxAmmo));
    }

    private IEnumerator addAmmo(int ammo)
    {
        yield return new WaitForSeconds(2f);

        isReloading = false;

        currentAmmo = ammo;

        Debug.Log("Reloaded!");
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
