using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponData : MonoBehaviour
{
    public Weapon weapon;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        if  (weapon != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = weapon.idleSprite;
        }
    }
}
