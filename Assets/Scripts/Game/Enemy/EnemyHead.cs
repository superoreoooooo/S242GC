using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHead : MonoBehaviour
{
    void Start()
    {

    }
    void Update()
    {
        /*
        //Vector2 HeadDir = GetComponentInParent<EnemyManager>().HeadDir;
        //float angleDeg = Mathf.Atan2(HeadDir.y, HeadDir.x) * Mathf.Rad2Deg;
        //transform.rotation = Quaternion.Euler(0, 0, GetComponentInParent<EnemyManager>().Flip ? angleDeg += 180f : angleDeg);

        Vector2 lookingDirection = GetComponentInParent<EnemyManager>().Flip ? -transform.right : transform.right;

        // 머리 위치에서 보는 방향으로 선 그리기
        Debug.DrawLine(transform.position, transform.position + (Vector3)lookingDirection * 5f, Color.red);
        //Debug.DrawLine(new Vector3(transform.position.x, transform.position.y, -3), new Vector3(transform.right.normalized.x * 5f, transform.right.normalized.y * 5f, -3), Color.cyan, 0.1f);
    */
    }
}
