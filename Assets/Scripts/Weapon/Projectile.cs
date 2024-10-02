using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody2D rb2d;

    public Vector2 direction;

    [SerializeField]
    private float projectileSpeed;

    [SerializeField]
    private float lifeTime;

    [SerializeField]
    private int damage;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        //rb2d.velocity = new Vector2(transform.forward.x, transform.forward.y) * projectileSpeed;
        rb2d.velocity = direction * projectileSpeed;

        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        //print(collision.gameObject.name);
        if (collision.gameObject.GetComponent<EnemyManager>() != null) {
            collision.gameObject.GetComponent<EnemyManager>().gainDamage(damage);
        }
        Destroy(gameObject);
    }
}
