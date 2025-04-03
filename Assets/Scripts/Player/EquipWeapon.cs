using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

public class EquipWeapon : MonoBehaviour
{
    [Header("Ray Settings")]
    [SerializeField][Range(0.0f, 2.0f)] private float rayLengt;
    [SerializeField] private Vector3 rayOffset; //för att flytta Ray uppåt så att den hamnar rätt med Fim
    [SerializeField] private LayerMask weaponMask; //för att determinera vad som kan bli träffat av rayen
    private RaycastHit topRayHitInfo;
    private RaycastHit bottomRayHitInfo;

    private RayGunScript currentWeapon;

    [SerializeField] private Transform equipPos;
    [SerializeField] private Transform shootingPos;

    private bool isShooting;

    [Header ("Right Hand Target")]
    [SerializeField] private TwoBoneIKConstraint rightHandIK; //Referens till höger handens IK constraint
    [SerializeField] private Transform rightHandTarget; //Referens till höger handens target, gjort för att kunna sätta vapnet i höger hand

    [Header("Left Hand Target")]
    [SerializeField] private TwoBoneIKConstraint leftHandIK; //Referens till vänster handens IK constraint
    [SerializeField] private Transform leftHandTarget; //Referens till vänster handens target, gjort för att kunna sätta vapnet i vänster hand

    [SerializeField] private Transform IKRightHandPos; //Referens till höger handens position, gjort för att kunna sätta vapnet i höger hand
    [SerializeField] private Transform IKLeftHandPos; //Referens till vänster handens position, gjort för att kunna sätta vapnet i vänster hand

    private bool IsEquipped;

    void Start()
    {
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Equip();
            Debug.Log("E pressed");
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            UnEquip();
        }

        if (currentWeapon)
        {
            if (!isShooting)
            {
                currentWeapon.transform.parent = equipPos.transform;
                currentWeapon.transform.position = equipPos.position;
                currentWeapon.transform.rotation = equipPos.rotation;

                leftHandIK.weight = 0f;
            }
            else
            {
                //currentWeapon.transform.parent = shootingPos.transform;
                //currentWeapon.transform.position = shootingPos.position;
                //currentWeapon.transform.rotation = shootingPos.rotation;

                leftHandIK.weight = 1f;
                leftHandTarget.position = IKLeftHandPos.position;
                leftHandTarget.rotation = IKLeftHandPos.rotation;

            }

            //rightHandIK.weight = 1f;
            //rightHandTarget.position = IKRightHandPos.position;
            //rightHandTarget.rotation = IKRightHandPos.rotation;
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
        Ray topRay = new Ray(transform.position + rayOffset, transform.forward);
        Ray bottomRay = new Ray(transform.position + Vector3.up * 0.175f, transform.forward);

        Debug.DrawRay(transform.position + rayOffset, transform.forward * rayLengt, Color.red);
        Debug.DrawRay(transform.position + Vector3.up * 0.175f, transform.forward * rayLengt, Color.green);

        Physics.Raycast(topRay, out topRayHitInfo, rayLengt, weaponMask); //För att kalla ut Rayen
        Physics.Raycast(bottomRay, out bottomRayHitInfo, rayLengt, weaponMask); //För att kalla ut Rayen
    }
    
    private void Equip()
    {
        RayCastHandler();

        if (topRayHitInfo.collider != null)
        {
            Debug.Log("Hit: " + topRayHitInfo.collider.name);
           if(topRayHitInfo.collider != null)
           {
                currentWeapon = topRayHitInfo.transform.GetComponent<RayGunScript>();
           }

            if (bottomRayHitInfo.collider)
            {
                currentWeapon = bottomRayHitInfo.transform.GetComponent<RayGunScript>();
            }

            if (!currentWeapon) return;

            //Sätter vapnets position och rotation till Player
         
            //får vapnet att sluta rotera
            currentWeapon.IsRotating = false;

            currentWeapon.gameObject.GetComponent<Collider>().enabled = false;

            currentWeapon.ChangeWeaponBehavior();

            IsEquipped = true;
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

            currentWeapon.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
            currentWeapon.GetComponent<Collider>().enabled = true;

            currentWeapon = null;
        }
    }
}
