using UnityEngine;

[CreateAssetMenu]
public class Weapon : ScriptableObject
{
    public string weaponName;
    public int damage;
    public int maxAmmo;  // Ammo per magazine
    public float fireRate;
    public Sprite idleSprite;
    public Sprite shootSprite;
    public Vector2 muzzleOffset;
}
