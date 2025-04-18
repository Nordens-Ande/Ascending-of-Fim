using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UIElements;

public class EnemyWeaponInventory : MonoBehaviour
{
    [SerializeField] EnemyShoot enemyShootScript;
    [SerializeField] DropWeaponScript dropWeaponScript;


    [Header("Prefab References")]
    [SerializeField] GameObject pistolPrefab;
    [SerializeField] GameObject riflePrefab; 
    [SerializeField] GameObject shotgunPrefab;

    [Header("WeaponPosition")]
    private Transform WeaponPosition;
    [SerializeField] private Transform pistolPos; //position where the weapon will be spawned, set in inspector
    [SerializeField] private Transform riflePos; //position where the weapon will be spawned, set in inspector
    [SerializeField] private Transform shotgunPos; //position where the weapon will be spawned, set in inspector
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
    bool enemyDead;

    GameObject weapon; //object for weapon that enemy will get, check method CreateWeapon() below
    enum Weapon { pistol, rifle, shotgun }
    WeaponData weaponData;
    WeaponScript weaponScript;

    void Start()
    {
        DecideWeapon();
        IsEquiped = false;
        enemyDead = false;
    }

    public void EnemyDead(bool e, bool d)
    {
        IsEquiped = e;
        enemyDead = d;
    }

    void DecideWeapon() //assign random weapon from possible ones above
    {
        Weapon[] weapons = (Weapon[])System.Enum.GetValues(typeof(Weapon));
        Weapon selectedWeapon = weapons[Random.Range(0, weapons.Length)];

        //GameObject weaponPrefab = null;
        //switch (selectedWeapon)
        //{
        //    case Weapon.pistol:
        //        weaponPrefab = pistolPrefab;
        //        break;
        //    case Weapon.rifle:
        //        weaponPrefab = riflePrefab; 
        //        break;
        //    case Weapon.shotgun:
        //        weaponPrefab = shotgunPrefab;
        //        break;
        //}

        GameObject weaponPrefab = riflePrefab;

        if (weaponPrefab != null)
        {
            CreateWeapon(weaponPrefab);
            RetrieveWeaponData();
            SetHandPos(weaponScript);
        }
    }

    void CreateWeapon(GameObject prefab)
    {
        string weaponName = prefab.GetComponent<WeaponScript>().GetWeaponData().weaponName.ToLower();
        if(weaponName == "pistol")
        {
            WeaponPosition = pistolPos;
        }
        else if (weaponName == "rifle")
        {
            WeaponPosition = riflePos;
        }
        else if (weaponName == "shotgun")
        {
            WeaponPosition = shotgunPos;
        }
        weapon = Instantiate(prefab, WeaponPosition.position, WeaponPosition.rotation, WeaponPosition);

    }

    void SetHandPos(WeaponScript weapon)
    {
        if(weapon != null)
        {
            IKLeftHandPos = weapon.LeftHand;
            IKRightHandPos = weapon.RightHand;
        }
    }

    void RetrieveWeaponData() // get data from weapon object and send to enemyShoot Script
    {
        if(weapon != null)
        {
            weaponScript = weapon.GetComponentInChildren<WeaponScript>();
            weaponScript.CheckIfWeaponBodyNull();
            weaponData = weaponScript.GetWeaponData();
            enemyShootScript.SetWeaponData(weaponData, weaponScript);
            dropWeaponScript.SetWeapon(weapon);
        }
    }

    public void UnEquip()
    {
        weapon.transform.parent = null;
        weaponScript.Unequip();
        IsEquiped = false;
        weapon = null;
        weaponScript = null;
        weaponData = null;
    }

    void Update()
    {
        if (IsEquiped && enemyDead == false)
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
        else
        {
            if(weapon != null)
            {
                weaponScript.Equip();
                IsEquiped = true;
            }
        }
    }
}
