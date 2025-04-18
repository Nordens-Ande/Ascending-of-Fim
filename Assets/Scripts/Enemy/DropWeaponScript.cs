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
            weaponScript = this.weapon.GetComponent<WeaponScript>();
        }
        else
        {
            Debug.Log("EnemyDropWeaponScript: weapon null after SetWeapon method");
        }
    }

    public void DropWeapon() // call when enemy dies
    {
        if(weaponScript == null)
        {
            Debug.Log("weaponScript null in dropWeapon");
        }
        weapon.transform.parent = null;
        weaponScript.Unequip();
    }
}
