using UnityEngine;
using UnityEngine.UIElements;

public class EnemyWeaponInventory : MonoBehaviour
{
    [SerializeField] EnemyShoot enemyShootScript;
    [SerializeField] DropWeaponScript dropWeaponScript;

    [Header("Prefab References")]
    [SerializeField] GameObject pistol;
    //[SerializeField] GameObject rifle; //uncomment when rifle/shotgun prefabs exist and assign in inspector
    //[SerializeField] GameObject shotgun;

    GameObject weapon; //object for weapon that enemy will get, check method CreateWeapon() below
    enum Weapon { pistol, rifle, shotgun }
    WeaponData weaponData;
    WeaponScript weaponScript;

    void Start()
    {
        DecideWeapon();
    }

    void DecideWeapon() //assign random weapon from possible ones above
    {
        //Weapon[] weapons = (Weapon[])System.Enum.GetValues(typeof(Weapon));
        //Weapon selectedWeapon = weapons[Random.Range(0, weapons.Length)];

        //GameObject weaponPrefab = null;
        //switch (selectedWeapon)
        //{
        //    case Weapon.pistol:
        //        weaponPrefab = pistol;
        //        break;
        //    case Weapon.rifle:
        //        //weaponPrefab = rifle; //uncomment when prefabs for rifle and shotgun exist
        //        break;
        //    case Weapon.shotgun:
        //        //weaponPrefab = shotgun;
        //        break;
        //}

        GameObject weaponPrefab = pistol;

        if(weaponPrefab != null)
        {
            CreateWeapon(weaponPrefab);
            RetrieveWeaponData();
        }
    }

    void CreateWeapon(GameObject prefab)
    {
        Vector3 spawn = new Vector3(transform.position.x, transform.position.y, transform.position.z + 10); // just for test feel free to change
        weapon = Instantiate(prefab, spawn, transform.rotation, transform);
    }

    void RetrieveWeaponData() // get data from weapon object and send to enemyShoot Script
    {
        weaponScript = weapon.GetComponentInChildren<WeaponScript>();
        weaponScript.CheckIfWeaponBodyNull();
        weaponScript.Equip();
        Debug.Log("weaponScript.Equip");
        weaponData = weaponScript.GetWeaponData();
        enemyShootScript.SetWeaponData(weaponData, weaponScript);
        dropWeaponScript.SetWeapon(weapon);
    }

    void Update()
    {
        
    }
}
