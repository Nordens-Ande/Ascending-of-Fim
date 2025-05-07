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

    [Header("AnimationPos")]
    [SerializeField] private float AnimationSpeed;
    [SerializeField] private Transform raygunPos;
    [SerializeField] private Transform pistolPos;
    [SerializeField] private Transform riflePos;
    [SerializeField] private Transform shotgunPos;
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

    void Start()
    {
        IsEquipped = false;
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
        
        if (currentWeapon != null)
        {
            //FUnkar ej med denna
            //if (!isShooting)
            //{
            //    //currentWeapon.transform.parent = equipPos.transform;
            //    //currentWeapon.transform.position = equipPos.position;
            //    //currentWeapon.transform.rotation = equipPos.rotation;

            //    currentWeapon.transform.parent = equipPos.transform; //h�r
            //    currentWeapon.transform.position = Vector3.Lerp(currentWeapon.transform.position, equipPos.position, Time.deltaTime * AnimationSpeed);
            //    currentWeapon.transform.rotation = Quaternion.Lerp(currentWeapon.transform.rotation, equipPos.rotation, Time.deltaTime * AnimationSpeed);

            //    leftHandIK.weight = 0f;
            //}
            if(IsEquipped)
            {
                //currentWeapon.transform.parent = shootingPos.transform;
                //currentWeapon.transform.position = shootingPos.position; //h�r
                //currentWeapon.transform.rotation = shootingPos.rotation;
                
                currentWeapon.transform.parent = WeaponPosition.transform; //h�r
                currentWeapon.transform.position = Vector3.Lerp(currentWeapon.transform.position, WeaponPosition.position, Time.deltaTime * AnimationSpeed); //test
                currentWeapon.transform.rotation = Quaternion.Lerp(currentWeapon.transform.rotation, WeaponPosition.rotation, Time.deltaTime * AnimationSpeed); 

                
                leftHandIK.weight = 1f;
                leftHandTarget.position = IKLeftHandPos.position;
                leftHandTarget.rotation = IKLeftHandPos.rotation;

                rightHandIK.weight = 1f;
                rightHandTarget.position = IKRightHandPos.position; //h�r
                rightHandTarget.rotation = IKRightHandPos.rotation;
            }
        }
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

    private void Equip()
    {
        RayCastHandler();

        if (topRayHitInfo.collider != null)
        {
            if (topRayHitInfo.collider != null)
            {
                if(IsEquipped)
                {
                    UnEquip();
                }
                currentWeapon = topRayHitInfo.transform.GetComponent<WeaponScript>();
                currentWeaponObject = topRayHitInfo.collider.gameObject;
            }

            if (currentWeapon == null) 
            {
                Debug.Log("currentWeapon null after equip for player");
                return;
            }

            SetHandPos(currentWeapon);
            SetWeaponPos();
            currentWeapon.Equip();
            IsEquipped = true;
        }
    }

    void SetWeaponPos()
    {
        string weaponName = currentWeapon.GetWeaponData().weaponName.ToLower();
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

    public void UnEquip() 
    {
        if (IsEquipped)
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
    }
}
