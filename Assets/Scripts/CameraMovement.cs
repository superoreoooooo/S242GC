using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    private GameObject target;
    
    [SerializeField]
    private float moveSpeed;


    void Start()
    {
        
    }

    void Update()
    {
        Vector3 tpos = new Vector3(target.transform.position.x, target.transform.position.y, -10);
        transform.position = Vector3.Lerp(transform.position, tpos, moveSpeed * Time.deltaTime);
    }
}
