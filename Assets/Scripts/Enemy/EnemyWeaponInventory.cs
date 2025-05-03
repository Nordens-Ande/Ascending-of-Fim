using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UIElements;

public class EnemyWeaponInventory : MonoBehaviour
{
    [SerializeField] EnemyShoot enemyShootScript;

    [Header("Prefab References")]
    [SerializeField] GameObject pistolPrefab;
    [SerializeField] GameObject riflePrefab; 
    [SerializeField] GameObject shotgunPrefab;

    [SerializeField] GameObject shieldPrefab;

    [Header("WeaponPosition")]
    private Transform WeaponPosition;
    [SerializeField] private Transform pistolPos; //position where the weapon will be spawned, set in inspector
    [SerializeField] private Transform pistolPosShield; //position where the weapon will be spawned if enemy has shield, set in inspector
    [SerializeField] private Transform riflePos; //position where the weapon will be spawned, set in inspector
    [SerializeField] private Transform shotgunPos; //position where the weapon will be spawned, set in inspector
    [SerializeField] private Transform shieldPos; //position where the shield will be spawned, set in inspector
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
    bool hasShield;
    bool enemyDead;

    GameObject weapon; //object for weapon that enemy will get, check method CreateWeapon() below
    GameObject shield;
    ShieldScript shieldScript;

    enum Weapon { pistol, rifle, shotgun }
    WeaponData weaponData;
    WeaponScript weaponScript;

    void Start()
    {
        hasShield = false;
        IsEquiped = false;
        enemyDead = false;
        RollForShield();
        if(hasShield == true)
        {
            CreateShield();
        }
        else
        {
            DecideWeapon();
        }
    }

    public void EnemyDead(bool e, bool d)
    {
        IsEquiped = e;
        enemyDead = d;
    }

    void RollForShield()
    {
        int random = Random.Range(1, 101);
        if (random < 15)
        {
            hasShield = true;
        }
    }

    void CreateShield()
    {
        WeaponPosition = pistolPosShield;
        weapon = Instantiate(pistolPrefab, WeaponPosition.position, WeaponPosition.rotation, WeaponPosition);
        shield = Instantiate(shieldPrefab, shieldPos.position, shieldPos.rotation, shieldPos);
        RetrieveWeaponData();
        RetrieveShieldScript();
        SetHandPos(weaponScript, shieldScript);
    }

    void DecideWeapon() //assign random weapon from possible ones above
    {
        Weapon[] weapons = (Weapon[])System.Enum.GetValues(typeof(Weapon));
        Weapon selectedWeapon = weapons[Random.Range(0, weapons.Length)];

        GameObject weaponPrefab = null;
        switch (selectedWeapon)
        {
            case Weapon.pistol:
                weaponPrefab = pistolPrefab;
                break;
            case Weapon.rifle:
                weaponPrefab = riflePrefab;
                break;
            case Weapon.shotgun:
                weaponPrefab = shotgunPrefab;
                break;
        }

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

    void SetHandPos(WeaponScript weapon) // without shield
    {
        if(weapon != null)
        {
            IKLeftHandPos = weapon.LeftHand;
            IKRightHandPos = weapon.RightHand;
        }
    }

    void SetHandPos(WeaponScript weapon, ShieldScript shield) // with shield
    {
        if(weapon != null && shield != null)
        {
            IKRightHandPos = weapon.RightHand;
            IKLeftHandPos = shield.HandPos;
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
            weaponScript.Equip();
        }
    }

    void RetrieveShieldScript()
    {
        if(shield != null)
        {
            shieldScript = shield.GetComponentInChildren<ShieldScript>();
            shieldScript.CheckIfShieldBodyNull();
            shieldScript.Equip();
        }
    }

    public void UnEquip()
    {
        if(hasShield)
        {
            if(shield != null)
            {
                shield.transform.parent = null;
                shieldScript.Unequip(true);
                hasShield = false;
                shield = null;
                shieldScript = null;
            }
        }
        if(weapon != null)
        {
            weapon.transform.parent = null;
            weaponScript.Unequip(true);
        }
        IsEquiped = false;
        weapon = null;
        weaponScript = null;
        weaponData = null;
    }

    void Update()
    {
        if (IsEquiped && enemyDead == false)
        {
            if(hasShield)
            {
                shield.transform.parent = shieldPos.transform;
                shield.transform.position = Vector3.Lerp(shield.transform.position, shieldPos.position, Time.deltaTime * AnimationSpeed); //test
                shield.transform.rotation = Quaternion.Lerp(shield.transform.rotation, shieldPos.rotation, Time.deltaTime * AnimationSpeed); //test
            }
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
            if(shield != null)
            {
                shieldScript.Equip();
                hasShield = true;
            }
        }
    }
}
