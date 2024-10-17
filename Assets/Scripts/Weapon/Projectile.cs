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
    
    //기본 설정. Rigidbody2D 컴포넌트를 가져와 속력 지정
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
    
    //충돌 시 오브젝트 삭제 (Trigger) 비-물리 충돌
    private void OnTriggerEnter2D(Collider2D collider)
    {
        Destroy(gameObject);
    }

    //충돌 시 오브젝트 삭제 (Collision) 물리 충돌
    private void OnCollisionEnter2D(Collision2D collision) {
        Destroy(gameObject);
    }
}
