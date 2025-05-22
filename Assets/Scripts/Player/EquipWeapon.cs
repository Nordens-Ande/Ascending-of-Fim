using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;
using static UnityEditor.Progress;

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
    }

    public void OnInteract(InputValue inputValue)
    {
        Equip();
        Debug.Log("pressed E");
    }


    public void OnDrop(InputValue inputValue)
    {
        UnEquip();
        Debug.Log("Pressed Q");
    }

    private void Update()
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

    private void RayCastHandler()
    {
        Ray topRay = new Ray(transform.position + rayOffset, orientationObject.forward);
        //Ray bottomRay = new Ray(transform.position + Vector3.up * 0.175f, orientationObject.forward);

        Debug.DrawRay(transform.position + rayOffset, orientationObject.forward * rayLengt, Color.red);
        //Debug.DrawRay(transform.position + Vector3.up * 0.175f, orientationObject.forward * rayLengt, Color.green);

        Physics.Raycast(topRay, out topRayHitInfo, rayLengt, weaponMask); //F�r att kalla ut Rayen
        //Physics.Raycast(bottomRay, out bottomRayHitInfo, rayLengt, weaponMask); //F�r att kalla ut Rayen
    }

    void SetHandPos(WeaponScript weapon)
    {
        IKLeftHandPos = weapon.LeftHand;
        IKRightHandPos = weapon.RightHand;
    }
    void SetHandPos(WeaponScript weapon, ShieldScript shield) // with shield
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

    private void Equip()
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
                        UnEquip();
                    }
                    currentWeapon = topRayHitInfo.transform.GetComponent<WeaponScript>();
                    currentWeaponObject = topRayHitInfo.collider.gameObject;
                    currentWeapon.Equip();
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

    void EquipShield(GameObject gameObject)
    {
        shield = gameObject;
        shieldScript = shield.GetComponent<ShieldScript>();
        shieldScript.SetOwner(this.gameObject);
        shieldScript.Equip();
        hasShield = true;
    }

    void SetWeaponPos()
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

    public void UnEquip() 
    {
        if(hasShield)
        {
            if (shield != null)
            {
                shield.transform.parent = null;
                shieldScript.Unequip(false);
                shieldScript.SetOwner(null);
                hasShield = false;
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
