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

    [SerializeField]
    private int knifeDamage;

    [SerializeField]
    private float knifeDistance;

    [SerializeField]
    private float knifeAngle;

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
        else if (Input.GetMouseButtonDown(1))
        {
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, knifeDistance);
            foreach (var collider in hitColliders)
            {
                if (collider.tag == "Enemy")
                {
                    Vector3 dir = (collider.transform.position - transform.position).normalized;

                    float angle = Vector3.Angle(direction, dir);
                    if (angle <= knifeAngle)
                    {
                        Debug.Log("HIT " + collider.name);
                        collider.gameObject.GetComponent<EnemyManager>().gainDamage(knifeDamage);
                    }
                }
            }

            LineRenderer lineRenderer = GetComponent<LineRenderer>();


            lineRenderer.startColor = Color.green;
            lineRenderer.endColor = Color.cyan;

            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.1f;

            lineRenderer.positionCount = 3;

            Vector2 startPosition = transform.position;

            // 좌우로 60도 각도의 벡터 계산 (2D 기준으로)
            float halfAngle = knifeAngle / 2f;

            // 좌측 방향 (direction에서 -60도 회전)
            Vector2 leftDirection = Quaternion.Euler(0, 0, -halfAngle) * direction;
            // 우측 방향 (direction에서 +60도 회전)
            Vector2 rightDirection = Quaternion.Euler(0, 0, halfAngle) * direction;

            // 각 방향으로의 끝점 계산 (길이 5)
            Vector2 leftEndPoint = startPosition + leftDirection.normalized * knifeDistance;
            Vector2 rightEndPoint = startPosition + rightDirection.normalized * knifeDistance;

            // LineRenderer로 원뿔의 양쪽 끝과 시작점을 그리기
            lineRenderer.SetPosition(0, leftEndPoint);   // 좌측 끝점
            lineRenderer.SetPosition(1, startPosition);  // 시작점 (플레이어 위치)
            lineRenderer.SetPosition(2, rightEndPoint);  // 우측 끝점
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
                    Physics2D.IgnoreLayerCollision(gameObject.layer, lm, ignore: true);
                    //Physics2D.IgnoreCollision(coll, GetComponent<Collider2D>(), true);
                    StartCoroutine(updateInvincible(lm));
                    StartCoroutine(updateSpriteBlur());
                }
                else
                {
                    collision.gameObject.GetComponent<EnemyManager>().gainDamage(movement.DashDamage);
                }
            }
        }
    }

    private IEnumerator updateInvincible(LayerMask lm)
    {
        yield return new WaitForSeconds(invincibleTime);
        isInvincible = false;
        Physics2D.IgnoreLayerCollision(gameObject.layer, lm, false);
    }

    private void Fade()
    {
        Color fc = sr.color;
        fc.a = 0f;
        sr.color = fc;
    }

    private void Sharp()
    {
        Color fc = sr.color;
        fc.a = 255f;
        sr.color = fc;
    }

    private IEnumerator updateSpriteBlur()
    {
        for (int i = 0; i < 5; i++)
        {
            Fade();
            yield return new WaitForSeconds(0.2f);

            Sharp();
            yield return new WaitForSeconds(0.2f);
        }
    }
}
