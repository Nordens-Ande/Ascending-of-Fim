using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Scriptable Objects/WeaponData")]
public class WeaponData : ScriptableObject
{
    public enum Type
    {
        Primary,
        Secondary,
        Throwable,
    }
    public Type weaponType;

    public string weaponName;
    public int damage;
    public float fireRate;
    public int ammoCapacity;
}
