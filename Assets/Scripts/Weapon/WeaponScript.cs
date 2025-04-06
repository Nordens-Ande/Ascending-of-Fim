using UnityEngine;

public interface IWeapon
{
    public WeaponData GetWeaponData();
}

public class WeaponScript : MonoBehaviour, IWeapon
{
    [SerializeField] WeaponData WeaponData;

    public WeaponData GetWeaponData()
    {
        return WeaponData;
    }
}
