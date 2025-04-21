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

    private GameObject currentWeaponObject;
    private WeaponScript currentWeapon;

    [Header("AnimationPos")]
    [SerializeField] private float AnimationSpeed;
    [SerializeField] private Transform equipPos;
    [SerializeField] private Transform shootingPos;

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
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Equip();
            Debug.Log("E pressed");
            Debug.Log("IsEquipped: " + IsEquipped);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            UnEquip();
        } 

        if (currentWeapon)
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
                
                currentWeapon.transform.parent = shootingPos.transform; //h�r
                currentWeapon.transform.position = Vector3.Lerp(currentWeapon.transform.position, shootingPos.position,Time.deltaTime* AnimationSpeed); //test
                currentWeapon.transform.rotation = Quaternion.Lerp(currentWeapon.transform.rotation, shootingPos.rotation, Time.deltaTime * AnimationSpeed); 

                
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
            Debug.Log("Hit: " + topRayHitInfo.collider.name);
            if (topRayHitInfo.collider != null)
            {
                currentWeapon = topRayHitInfo.transform.GetComponent<WeaponScript>();
                SetHandPos(currentWeapon);
            }

            if (!currentWeapon) return;

            currentWeapon.Equip();

            //currentWeapon.gameObject.GetComponent<Collider>().enabled = false;

            //currentWeapon.ChangeWeaponBehavior();

            IsEquipped = true;

            IInventory inventory = InventoryReference.GetComponent<IInventory>();
            if (inventory == null)
                return;

            inventory.ChangeEquipped(currentWeapon.gameObject);
        }
    }
    private void UnEquip() 
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

            //currentWeapon.Unequip();
            Destroy(currentWeapon.gameObject);

            currentWeapon = null;
            
        }
    }
}
