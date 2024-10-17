using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHealthBar : MonoBehaviour
{
    public Transform player;

    //업데이트. 체력바 플레이어 위치 근처로 이동 
    void Update()
    {
        transform.position = new Vector2(player.position.x, player.position.y + 0.86f);
    }
}
