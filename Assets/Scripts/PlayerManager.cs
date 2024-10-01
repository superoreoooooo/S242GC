using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private LineRenderer lr;
    private float lrLen = 3f;

    [SerializeField]
    private GameObject projectile;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = Color.red;
        lr.endColor = Color.blue;
        lr.positionCount = 2;
    }

    void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        Vector2 direction = (mousePos - transform.position).normalized;

        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, transform.position + new Vector3(direction.x, direction.y, transform.position.z) * lrLen);

        if (Input.GetMouseButtonDown(0)) {
            GameObject obj = Instantiate(projectile, transform.position, Quaternion.identity);
            Projectile pj = obj.GetComponent<Projectile>();
            pj.direction = direction;
        }
    }
}
