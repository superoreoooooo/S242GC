using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("Gun Settings")]
    public int damage = 10;
    public int bulletCount = 30;
    public float fireRate = 1f;
    public Sprite idleSprite;
    public Sprite shootingSprite;

    [Header("References")]
    public Vector2 dir;

    private SpriteRenderer spriteRenderer;

    protected float nextFireTime = 0f;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = idleSprite;
        }
    }

    public virtual void Shoot()
    {
        if (Time.time >= nextFireTime && bulletCount > 0)
        {
            nextFireTime = Time.time + 1f / fireRate;
            bulletCount--;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir);
            if (hit.collider != null)
            {
                Debug.Log("Hit " + hit.collider.name);
                hit.collider.GetComponent<EnemyManager>().gainDamage(damage);
            }

            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = shootingSprite;
            }

            Debug.Log("Shot fired from " + gameObject.name);
            Invoke("ResetSprite", 0.1f);
        }
        else if (bulletCount <= 0)
        {
            Debug.Log("Out of bullets");
        }
    }

    public void Reload(int ammoAmount)
    {
        bulletCount += ammoAmount;
        Debug.Log("Reloaded " + ammoAmount + " bullets. Current bullet count: " + bulletCount);
    }

    void ResetSprite()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = idleSprite;
        }
    }
}
