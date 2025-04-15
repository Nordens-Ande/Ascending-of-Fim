using UnityEngine;

public class DropWeaponScript : MonoBehaviour
{
    GameObject weapon;
    WeaponScript weaponScript;

    public void SetWeapon(GameObject weapon)
    {
        this.weapon = weapon;
        if(this.weapon != null)
        {
            weaponScript = weapon.GetComponent<WeaponScript>();
        }
        else
        {
            Debug.Log("EnemyDropWeaponScript: weapon null after SetWeapon method");
        }
    }

    public void DropWeapon() // call when enemy dies
    {
        weaponScript.Unequip();
    }
}
