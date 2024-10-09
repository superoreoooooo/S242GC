using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class WeaponManager : MonoBehaviour
{
    public Weapon currentWeapon;   // Reference to the current weapon ScriptableObject
    public LayerMask hitLayers;    // Layers that the ray can hit

    private int currentAmmo;
    private float nextTimeToFire = 0f;

    public Light2D muzzleFlash;

    private void Start()
    {
        currentAmmo = currentWeapon.maxAmmo;
        muzzleFlash.enabled = false;
    }

    private void Update()
    {
        UpdateFirePointPosition();
        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
        {
            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }


        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        Vector2 direction = (mousePos - transform.position).normalized;

        LookAtDirection2D(direction);
    }
    void LookAtDirection2D(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    void Shoot()
    {
        if (currentAmmo <= 0)
        {
            Debug.Log("Out of ammo, reload!");
            return;
        }

        nextTimeToFire = Time.time + 1f / currentWeapon.fireRate;
        currentAmmo--;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        Vector2 direction = (mousePos - transform.position).normalized;

        // Raycast for shooting
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, Mathf.Infinity, hitLayers);

        // Debug Ray to visualize the shooting
        Debug.DrawRay(transform.position, direction * 100, Color.red, 1f);

        muzzleFlash.enabled = true;

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

        StartCoroutine(DisableMuzzleFlash());
    }

    private IEnumerator DisableMuzzleFlash()
    {
        yield return new WaitForSeconds(0.2f);
        muzzleFlash.enabled = false;
    }


    void Reload()
    {
        Debug.Log("Reloading...");
        currentAmmo = currentWeapon.maxAmmo;
    }

    void ResetSprite()
    {
        GetComponent<SpriteRenderer>().sprite = currentWeapon.idleSprite;
    }
    void UpdateFirePointPosition()
    {
        muzzleFlash.transform.localPosition = currentWeapon.muzzleOffset;
    }
}
