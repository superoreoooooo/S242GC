using UnityEditor.Animations;
using UnityEngine;

[CreateAssetMenu]
public class Weapon : ScriptableObject
{
    public string weaponName;
    public int damage;
    public int maxAmmo;  // Ammo per magazine
    public float fireRate;
    public AnimatorController weaponAnimator;
    public Sprite idleSprite;
    public Vector2 muzzleOffset;
    public float gunSoundDistance;
    public AudioClip weaponSound;
    public GameObject prefab;
}
