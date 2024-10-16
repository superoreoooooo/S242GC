using UnityEngine;

[CreateAssetMenu]
public class Weapon : ScriptableObject
{
    public string weaponName;
    public int damage;
    public int maxAmmo;  // Ammo per magazine
    public float fireRate;
    public RuntimeAnimatorController weaponAnimator;
    public Sprite idleSprite;
    public Vector2 muzzleOffset;
    public float gunSoundDistance;
    public AudioClip weaponSound;
    public GameObject prefab;
}
