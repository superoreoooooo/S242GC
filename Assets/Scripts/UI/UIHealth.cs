using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIHealth : MonoBehaviour
{
    public GameObject player;
    private TextMeshProUGUI tmpUGUI;

    //시작 시 설정
    void Start()
    {
        tmpUGUI = GetComponent<TextMeshProUGUI>();
    }

    //체력 UI 텍스트 업데이트 
    void Update()
    {
        int health = player.GetComponent<PlayerManager>().Health;
        tmpUGUI.text = "Health : " + health;
    }
}
