using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerManager : MonoBehaviour
{
    /*
    private LineRenderer lr;
    private float lrLen = 3f;
    */

    public UnityEvent onPlayerDead;
    private PlayerMovement movement;

    [SerializeField]
    private GameObject projectile;

    [SerializeField]
    private int health;

    public int Health
    {
        get => health;
    }

    [SerializeField]
    private float invincibleTime;

    private bool isInvincible;

    private void initPlayer()
    {
        //health = 0;
        isInvincible = false;
    }

    private SpriteRenderer sr;

    private void updatePlayer()
    {
        if (health <= 0)
        {
            dead();
        }
    }

    private void dead()
    {
        onPlayerDead.Invoke();
        gameObject.SetActive(false);
        print("DIE!");
    }

    public void gainDamage(int amount)
    {
        health -= amount;
    }

    void Start()
    {
        initPlayer();

        movement = GetComponent<PlayerMovement>();
        sr = GetComponent<SpriteRenderer>();
        /*
        lr = GetComponent<LineRenderer>();
        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = Color.red;
        lr.endColor = Color.blue;
        lr.positionCount = 2; 
        */
    }

    void Update()
    {
        updatePlayer();
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        Vector2 direction = (mousePos - transform.position).normalized;

        /*
        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, transform.position + new Vector3(direction.x, direction.y, transform.position.z) * lrLen);
        */

        if (Input.GetMouseButtonDown(0))
        {
            GameObject obj = Instantiate(projectile, transform.position, Quaternion.identity);
            Projectile pj = obj.GetComponent<Projectile>();
            pj.direction = direction;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isInvincible)
        {
            if (collision.gameObject.tag == "Enemy")
            {
                if (!movement.IsDashing)
                {
                    health -= 1;
                    isInvincible = true;
                    LayerMask lm = collision.gameObject.layer;
                    Physics2D.IgnoreLayerCollision(gameObject.layer, lm, ignore : true);
                    //Physics2D.IgnoreCollision(coll, GetComponent<Collider2D>(), true);
                    StartCoroutine(updateInvincible(lm));
                    StartCoroutine(updateSpriteBlur());
                }
                else
                {
                    collision.gameObject.GetComponent<EnemyManager>().Kill();
                }
            }
        }
    }

    private IEnumerator updateInvincible(LayerMask lm) {
        yield return new WaitForSeconds(invincibleTime);
        isInvincible = false;
        Physics2D.IgnoreLayerCollision(gameObject.layer, lm, false);
    }

    private void Fade() {
        Color fc = sr.color;
        fc.a = 20f;
        sr.color = fc;

        print("F");
    }

    private void Sharp() {
        Color fc = sr.color;
        fc.a = 255f;
        sr.color = fc;

        print("S");
    }

    private IEnumerator updateSpriteBlur() {
        for (int i = 0; i < 5; i++) {
            Fade();
            yield return new WaitForSeconds(0.2f);

            Sharp();
            yield return new WaitForSeconds(0.2f);
        }
    }
}
