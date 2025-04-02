using UnityEngine;

public class EquipWeapon : MonoBehaviour
{
    [Header("Ray Settings")]
    [SerializeField][Range(0.0f, 2.0f)] private float rayLengt;
    [SerializeField] private Vector3 rayOffset; //f�r att flytta Ray upp�t s� att den hamnar r�tt med Fim
    [SerializeField] private LayerMask weaponMask; //f�r att determinera vad som kan bli tr�ffat av rayen
    private RaycastHit topRayHitInfo;

    private RayGunScript currentWeapon;

    [SerializeField] private Transform currentWeaponPos;


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
    }

    void FixedUpdate()
    {
        RayCastHandler();
    }
    private void RayCastHandler()
    {
        Ray topRay = new Ray(transform.position + rayOffset, transform.forward);

        Debug.DrawRay(transform.position + rayOffset, transform.forward * rayLengt, Color.red);

        Physics.Raycast(topRay, out topRayHitInfo, rayLengt, weaponMask); //F�r att kalla ut Rayen
    }

    private void Equip()
    {
        if(topRayHitInfo.collider != null)
        {
            Debug.Log("Hit: " + topRayHitInfo.collider.name);
            currentWeapon = topRayHitInfo.collider.GetComponent<RayGunScript>();

            if (!currentWeapon) return;

            //S�tter vapnets position och rotation till Player
            currentWeapon.transform.parent = currentWeaponPos.transform;
            currentWeapon.transform.position = currentWeaponPos.position;
            currentWeapon.transform.rotation = currentWeaponPos.rotation;

            //f�r vapnet att sluta rotera
            currentWeapon.isRotating = false;

        }
    }
}
