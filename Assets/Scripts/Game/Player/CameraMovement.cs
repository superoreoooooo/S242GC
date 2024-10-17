using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    private GameObject target;
    
    [SerializeField]
    private float moveSpeed;

    //매 프레임 업데이트. Vector3.Lerp로 부드러운 카메라 이동 구현
    void Update()
    {
        Vector3 tpos = new Vector3(target.transform.position.x, target.transform.position.y, -10);
        transform.position = Vector3.Lerp(transform.position, tpos, moveSpeed * Time.deltaTime);
    }
}
