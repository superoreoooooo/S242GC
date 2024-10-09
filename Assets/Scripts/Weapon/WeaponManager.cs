using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public Weapon currentWeapon;   // Reference to the current weapon ScriptableObject
    public Transform firePoint;    // The point where the raycast will originate
    public LayerMask hitLayers;    // Layers that the ray can hit

    private int currentAmmo;
    private float nextTimeToFire = 0f;

    private void Start()
    {
        currentAmmo = currentWeapon.maxAmmo;
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

        // Raycast for shooting
        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, firePoint.right, Mathf.Infinity, hitLayers);

        // Debug Ray to visualize the shooting
        Debug.DrawRay(firePoint.position, firePoint.right * 100, Color.red, 1f);

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
}
