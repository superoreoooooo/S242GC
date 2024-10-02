using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIHealth : MonoBehaviour
{
    public GameObject player;
    private TextMeshProUGUI tmpUGUI;

    void Start()
    {
        tmpUGUI = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        int health = player.GetComponent<PlayerManager>().Health;
        tmpUGUI.text = "Health : " + health;
    }
}
