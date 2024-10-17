using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponData : MonoBehaviour
{
    public Weapon weapon;

    [SerializeField]
    private SpriteRenderer spriteRenderer;
    
    //무기 스프라이트 초기값 설정
    private void Start()
    {
        if  (weapon != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = weapon.idleSprite;
        }
    }
}
