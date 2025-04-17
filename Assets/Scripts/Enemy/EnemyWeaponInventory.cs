using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UIElements;

public class EnemyWeaponInventory : MonoBehaviour
{
    [SerializeField] EnemyShoot enemyShootScript;
    [SerializeField] DropWeaponScript dropWeaponScript;


    [Header("Prefab References")]
    [SerializeField] GameObject pistol;
    [SerializeField] GameObject rifle; //uncomment when rifle/shotgun prefabs exist and assign in inspector
    //[SerializeField] GameObject shotgun;

    [Header("WeaponPosition")]
    [SerializeField] private Transform WeaponPosition; //position where the weapon will be spawned, set in inspector
    [SerializeField] private float AnimationSpeed;

    [Header("Right Hand Target")]
    [SerializeField] private TwoBoneIKConstraint RightHandIK;
    [SerializeField] private Transform RightHandTarget;

    [Header("Left Hand Target")]
    [SerializeField] private TwoBoneIKConstraint LeftHandIK;
    [SerializeField] private Transform LeftHandTarget;

    [SerializeField] private Transform IKRightHandPos;  //Referens till h�ger handens position, gjort f�r att kunna s�tta vapnet i h�ger hand
    [SerializeField] private Transform IKLeftHandPos; //Referens till v�nster handens position, gjort f�r att kunna s�tta vapnet i v�nster hand

    private bool IsEquiped; 

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
        Weapon[] weapons = (Weapon[])System.Enum.GetValues(typeof(Weapon));
        Weapon selectedWeapon = weapons[Random.Range(0, weapons.Length)];

        //GameObject weaponPrefab = null;
        //switch (selectedWeapon)
        //{
        //    case Weapon.pistol:
        //        weaponPrefab = pistol;
        //        break;
        //    case Weapon.rifle:
        //        weaponPrefab = rifle; //uncomment when prefabs for rifle and shotgun exist
                //break;
                //    case Weapon.shotgun:
                //        //weaponPrefab = shotgun;
                //        break;
        //}

        GameObject weaponPrefab = rifle;

        if (weaponPrefab != null)
        {
            CreateWeapon(weaponPrefab);
            RetrieveWeaponData();
            SetHandPos(weaponScript);
            IsEquiped = true;
        }
    }

    void CreateWeapon(GameObject prefab)
    {
        
        weapon = Instantiate(prefab, WeaponPosition.position ,transform.rotation, WeaponPosition);

    }

    void SetHandPos(WeaponScript weapon)
    {
        IKLeftHandPos = weapon.LeftHand;
        IKRightHandPos = weapon.RightHand;
        Debug.Log("IKLeftHandPos: " + IKLeftHandPos.position);
        Debug.Log("IKRightHandPos: " + IKRightHandPos.position);
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
        if (IsEquiped)
        {
            weapon.transform.parent = WeaponPosition.transform; //set the weapon to the position of the weapon position object
            weapon.transform.position = Vector3.Lerp(weapon.transform.position, WeaponPosition.position, Time.deltaTime * AnimationSpeed); //test
            weapon.transform.rotation = Quaternion.Lerp(weapon.transform.rotation, WeaponPosition.rotation, Time.deltaTime * AnimationSpeed); //test

            LeftHandIK.weight = 1f;
            LeftHandTarget.position = IKLeftHandPos.position;
            LeftHandTarget.rotation = IKLeftHandPos.rotation;

            RightHandIK.weight = 1f;
            RightHandTarget.position = IKRightHandPos.position; //h�r
            RightHandTarget.rotation = IKRightHandPos.rotation;
        }
    }
}
