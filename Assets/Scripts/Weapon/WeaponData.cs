using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Scriptable Objects/WeaponData")]
public class WeaponData : ScriptableObject
{
    public enum Type
    {
        RayGun,
        Pistol,
        Rifle,
        Shotgun
    }
    public Type weaponType;

    public string weaponName;
    public int damage;
    public float fireRate;
    public int ammoCapacity;
    public bool allowButtonHold;
    public float reloadTime;
}
