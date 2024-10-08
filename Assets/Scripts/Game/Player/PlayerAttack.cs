using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField]
    private float hitboxWidth;
    [SerializeField]
    private float hitboxHeight;
    [SerializeField]
    private float hitboxOffset;
    [SerializeField]
    private int attackDamage;

    void Update()
    {
        if (Input.GetMouseButtonDown(1)) 
        {
            Attack();
        }
    }

    Vector2 GetMouseDirection()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - transform.position).normalized;
        return direction;
    }

    void Attack()
    {
        Vector2 attackDirection = GetMouseDirection();
        Vector2 attackPosition = (Vector2)transform.position + attackDirection * hitboxOffset;

        Vector2 hitboxSize = new Vector2(hitboxWidth, hitboxHeight);
        float angle = Mathf.Atan2(attackDirection.y, attackDirection.x) * Mathf.Rad2Deg;

        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(attackPosition, hitboxSize, angle);

        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                enemy.GetComponent<EnemyManager>().gainDamage(attackDamage);
            }
        }
    }

    void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Vector2 attackDirection = GetMouseDirection();
            Vector2 attackPosition = (Vector2)transform.position + attackDirection * hitboxOffset;

            Gizmos.color = Color.red;
            Gizmos.matrix = Matrix4x4.TRS(attackPosition, Quaternion.Euler(0, 0, Mathf.Atan2(attackDirection.y, attackDirection.x) * Mathf.Rad2Deg), Vector3.one);
            Gizmos.DrawWireCube(Vector2.zero, new Vector2(hitboxWidth, hitboxHeight));
        }
    }
}
