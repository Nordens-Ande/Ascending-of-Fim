using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;
//using static UnityEditor.Progress;

public class EquipWeapon : MonoBehaviour
{
    [Header("Inventory")]
    [SerializeField] public PlayerInventory InventoryReference;

    [Header("Ray Settings")]
    [SerializeField][Range(0.0f, 2.0f)] private float rayLengt;
    [SerializeField] private Vector3 rayOffset; //f�r att flytta Ray upp�t s� att den hamnar r�tt med Fim
    [SerializeField] private LayerMask weaponMask; //f�r att determinera vad som kan bli tr�ffat av rayen
    [SerializeField] public Transform orientationObject;
    private RaycastHit topRayHitInfo;
    //private RaycastHit bottomRayHitInfo;

    public GameObject currentWeaponObject;
    private WeaponScript currentWeapon;

    GameObject shield;
    ShieldScript shieldScript;

    [Header("Prefab References")]
    [SerializeField] GameObject pistolPrefab;
    [SerializeField] GameObject raygunPrefab;
    [SerializeField] GameObject riflePrefab;
    [SerializeField] GameObject shotgunPrefab;

    [SerializeField] GameObject shieldPrefab;

    [Header("AnimationPos")]
    [SerializeField] private float AnimationSpeed;
    [SerializeField] private Transform raygunPos;
    [SerializeField] private Transform raygunPosShield;
    [SerializeField] private Transform pistolPos;
    [SerializeField] private Transform pistolPosShield;
    [SerializeField] private Transform riflePos;
    [SerializeField] private Transform shotgunPos;
    [SerializeField] private Transform shieldPos;
    Transform WeaponPosition;

    private bool isShooting;

    [Header ("Right Hand Target")]
    [SerializeField] private TwoBoneIKConstraint rightHandIK; //Referens till h�ger handens IK constraint
    [SerializeField] private Transform rightHandTarget; //Referens till h�ger handens target, gjort f�r att kunna s�tta vapnet i h�ger hand

    [Header("Left Hand Target")]
    [SerializeField] private TwoBoneIKConstraint leftHandIK; //Referens till v�nster handens IK constraint
    [SerializeField] private Transform leftHandTarget; //Referens till v�nster handens target, gjort f�r att kunna s�tta vapnet i v�nster hand

    [SerializeField] private Transform IKRightHandPos; //Referens till h�ger handens position, gjort f�r att kunna s�tta vapnet i h�ger hand
    [SerializeField] private Transform IKLeftHandPos; //Referens till v�nster handens position, gjort f�r att kunna s�tta vapnet i v�nster hand

    public bool IsEquipped;
    bool hasShield;

    void Start()
    {
        IsEquipped = false;
        hasShield = false;
        weaponMask = (LayerMask.GetMask("ShieldIgnore") | LayerMask.GetMask("Weapon"));
        CheckForWeaponOnSpawn();
    }

    public void CheckForWeaponOnSpawn() // called at the start of a level to transfer over weapons and shield from previous level.
    {
        bool spawnShield = false;
        bool spawnWeapon = false;
        if (PlayerStats.hasShield == true)
        {
            spawnShield = true;
        }
        if (!string.IsNullOrEmpty(PlayerStats.weapon))
        {
            spawnWeapon = true;
        }
        
        CreateSpawnWeapon(spawnShield, spawnWeapon);
    }

    void CreateSpawnWeapon(bool shield, bool weapon) // if the player had weapons/shield on previous level, instantiate them here
    {
        if (shield == true)
        {
            this.shield = Instantiate(shieldPrefab, shieldPos.position, shieldPos.rotation, shieldPos);
            shieldScript = this.shield.GetComponent<ShieldScript>();
            shieldScript.SetOwner(this.gameObject);
            shieldScript.CheckIfShieldBodyNull();
            shieldScript.Equip();
            hasShield = true;
        }
        if (weapon == true)
        {
            GameObject prefabToSpawn = null;
            string weaponName = PlayerStats.weapon;
            if(hasShield == true)
            {
                if (weaponName == "pistol")
                {
                    prefabToSpawn = pistolPrefab;
                    WeaponPosition = pistolPosShield;
                }
                else if (weaponName == "raygun")
                {
                    prefabToSpawn = raygunPrefab;
                    WeaponPosition = raygunPosShield;
                }
            }
            else
            {
                if (weaponName == "pistol")
                {
                    prefabToSpawn = pistolPrefab;
                    WeaponPosition = pistolPos;
                }
                else if (weaponName == "raygun")
                {
                    prefabToSpawn = raygunPrefab;
                    WeaponPosition = raygunPos;
                }
                else if (weaponName == "rifle")
                {
                    prefabToSpawn = riflePrefab;
                    WeaponPosition = riflePos;
                }
                else if (weaponName == "shotgun")
                {
                    prefabToSpawn = shotgunPrefab;
                    WeaponPosition = shotgunPos;
                }
            }

            currentWeaponObject = Instantiate(prefabToSpawn, WeaponPosition.position, WeaponPosition.rotation, WeaponPosition);
            currentWeapon = currentWeaponObject.GetComponent<WeaponScript>();
            currentWeapon.Initialized();
            currentWeapon.CheckIfWeaponBodyNull();
            currentWeapon.Equip();
            IsEquipped = true;
        }
        if (shieldScript != null)
        {
            SetHandPos(currentWeapon, shieldScript);
        }
        else if(currentWeapon != null && shieldScript == null)
        {
            SetHandPos(currentWeapon);
        }
    }

    public void OnInteract(InputValue inputValue)
    {
        Equip();
    }


    public void OnDrop(InputValue inputValue)
    {
        UnEquip(true);
    }

    private void Update() // lerp the shield and weapons to correct positions, also check if shield should get destroyed
    {
        if (hasShield)
        {
            if(shield != null)
            {
                shield.transform.parent = shieldPos.transform;
                shield.transform.position = Vector3.Lerp(shield.transform.position, shieldPos.position, Time.deltaTime * AnimationSpeed);
                shield.transform.rotation = Quaternion.Lerp(shield.transform.rotation, shieldPos.rotation, Time.deltaTime * AnimationSpeed);
            }

            if (shieldScript.GetHealth() <= 0)
            {
                PlayerStats.hasShield = false;
                shield.transform.parent = null;
                shieldScript.Unequip(true);
                shieldScript.SetOwner(null);
                Destroy(shield);
                hasShield = false;
                shield = null;
                shieldScript = null;
                if (currentWeapon != null)
                {
                    SetHandPos(currentWeapon);
                    SetWeaponPos();
                }
            }
        }

        if (IsEquipped)
        {
            if(currentWeapon != null)
            {
                currentWeapon.transform.parent = WeaponPosition.transform; //h�r
                currentWeapon.transform.position = Vector3.Lerp(currentWeapon.transform.position, WeaponPosition.position, Time.deltaTime * AnimationSpeed); //test
                currentWeapon.transform.rotation = Quaternion.Lerp(currentWeapon.transform.rotation, WeaponPosition.rotation, Time.deltaTime * AnimationSpeed);
            }
        }

        if(IsEquipped || hasShield)
        {
            leftHandIK.weight = 1f;
            leftHandTarget.position = IKLeftHandPos.position;
            leftHandTarget.rotation = IKLeftHandPos.rotation;

            rightHandIK.weight = 1f;
            rightHandTarget.position = IKRightHandPos.position; //h�r
            rightHandTarget.rotation = IKRightHandPos.rotation;
        }
        weaponMask = (LayerMask.GetMask("ShieldIgnore") | LayerMask.GetMask("Weapon"));
    }

    private void OnAttack(InputValue inputValue)
    {
        isShooting = true;
    }

    private void OnAttackStop(InputValue inputValue)
    {
        isShooting = false;
    }

    private void RayCastHandler() // build ray cast for the equip method
    {
        Ray topRay = new Ray(transform.position + rayOffset, orientationObject.forward);
        //Ray bottomRay = new Ray(transform.position + Vector3.up * 0.175f, orientationObject.forward);

        Debug.DrawRay(transform.position + rayOffset, orientationObject.forward * rayLengt, Color.red);
        //Debug.DrawRay(transform.position + Vector3.up * 0.175f, orientationObject.forward * rayLengt, Color.green);

        Physics.Raycast(topRay, out topRayHitInfo, rayLengt, weaponMask); //F�r att kalla ut Rayen
        //Physics.Raycast(bottomRay, out bottomRayHitInfo, rayLengt, weaponMask); //F�r att kalla ut Rayen
    }

    void SetHandPos(WeaponScript weapon) // get reference to where the player should position their hands from the weapon
    {
        IKLeftHandPos = weapon.LeftHand;
        IKRightHandPos = weapon.RightHand;
    }
    void SetHandPos(WeaponScript weapon, ShieldScript shield) // get reference to where the player should position their hands from the weapon but also if a shield is equipped
    {
        if(weapon != null && shield != null)
        {
            IKRightHandPos = weapon.RightHand;
            IKLeftHandPos = shield.HandPos;
        }
        else if(weapon == null)
        {
            IKRightHandPos = shield.HandPos;
            IKLeftHandPos = shield.HandPos;
        }
    }

    private void Equip() // equip a new weapon or shield, logic with what weapons can be picked up if you carry a shield aswell as if a shield can be picked up.
    {
        RayCastHandler();

        if (topRayHitInfo.collider != null)
        {
            string[] acceptableWeapons; // what weapons can be picked up, based on if the player has shield or not
            if (hasShield)
            {
                acceptableWeapons = new string[] { "RayGun", "Pistol" };
            }
            else
            {
                acceptableWeapons = new string[] { "RayGun", "Pistol", "Rifle", "Shotgun" };
            }

            if (topRayHitInfo.collider.CompareTag("Weapon"))
            {
                if(acceptableWeapons.Contains(topRayHitInfo.collider.GetComponent<WeaponScript>().GetWeaponData().weaponName))
                {
                    if (IsEquipped)
                    {
                        UnEquip(false);
                    }
                    currentWeapon = topRayHitInfo.transform.GetComponent<WeaponScript>();
                    currentWeaponObject = topRayHitInfo.collider.gameObject;
                    currentWeapon.Equip();
                    PlayerStats.weapon = currentWeapon.GetWeaponData().weaponName.ToLower();
                    IsEquipped = true;
                } 
            }
            else if(topRayHitInfo.collider.CompareTag("Shield"))
            {
                if(IsEquipped && currentWeapon != null)
                {
                    if(currentWeapon.GetWeaponData().weaponName == "Pistol" || currentWeapon.GetWeaponData().weaponName == "RayGun") // make sure we can only pick up shield if we have pistol or raygun
                    {
                        EquipShield(topRayHitInfo.collider.gameObject);
                    }
                }
                else if(IsEquipped == false) // if we dont have weapon we can equip shield
                {
                    EquipShield(topRayHitInfo.collider.gameObject);
                }
            }

            if(!hasShield)
            {
                SetHandPos(currentWeapon);
            }
            else
            {
                SetHandPos(currentWeapon, shieldScript);
            }
            SetWeaponPos();
        }
    }

    void EquipShield(GameObject gameObject) //get reference to the shield object and change shield behaviour (from on ground rotating to equipped)
    {
        shield = gameObject;
        shieldScript = shield.GetComponent<ShieldScript>();
        shieldScript.SetOwner(this.gameObject);
        shieldScript.Equip();
        hasShield = true;
        PlayerStats.hasShield = true;
    }

    void SetWeaponPos() // decide what weapon position to use based on what weapon equipped.
    {
        if(currentWeapon != null)
        {
            string weaponName = currentWeapon.GetWeaponData().weaponName.ToLower();
            if (hasShield)
            {
                if (weaponName == "raygun")
                {
                    WeaponPosition = raygunPosShield;
                }
                else if (weaponName == "pistol")
                {
                    WeaponPosition = pistolPosShield;
                }
            }
            else
            {
                if (weaponName == "raygun")
                {
                    WeaponPosition = raygunPos;
                }
                else if (weaponName == "pistol")
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
            }
        }
    }

    public void UnEquip(bool dropShield) //unequip shield if a shield is equipped, if no shield then unequip the weapon
    {
        if(dropShield && hasShield)
        {
            if (shield != null)
            {
                shield.transform.parent = null;
                shieldScript.Unequip(false);
                shieldScript.SetOwner(null);
                hasShield = false;
                PlayerStats.hasShield = false;
                shield = null;
                shieldScript = null;
                if(currentWeapon != null)
                {
                    SetHandPos(currentWeapon);
                    SetWeaponPos();
                }
            }
        }
        else if (IsEquipped)
        {
            rightHandIK.weight = 0.0f;

            if (IKLeftHandPos)
            {
                leftHandIK.weight = 0.0f;
            }

            PlayerStats.weapon = null;
            IsEquipped = false;

            currentWeapon.transform.parent = null;

            currentWeapon.Unequip(false);

            currentWeapon = null;
            currentWeaponObject = null;
        }

        if (!IsEquipped && !hasShield)
        {
            rightHandIK.weight = 0.0f;
            leftHandIK.weight = 0.0f;
        }
    }
}
