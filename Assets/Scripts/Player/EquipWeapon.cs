using UnityEngine;

public class EquipWeapon : MonoBehaviour
{
    [Header("Ray Settings")]
    [SerializeField][Range(0.0f, 2.0f)] private float rayLengt;
    [SerializeField] private Vector3 rayOffset; //för att flytta Ray uppåt så att den hamnar rätt med Fim
    [SerializeField] private LayerMask weaponMask; //för att determinera vad som kan bli träffat av rayen
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

        Physics.Raycast(topRay, out topRayHitInfo, rayLengt, weaponMask); //För att kalla ut Rayen
    }

    private void Equip()
    {
        if(topRayHitInfo.collider != null)
        {
            Debug.Log("Hit: " + topRayHitInfo.collider.name);
            currentWeapon = topRayHitInfo.collider.GetComponent<RayGunScript>();

            if (!currentWeapon) return;

            //Sätter vapnets position och rotation till Player
            currentWeapon.transform.parent = currentWeaponPos.transform;
            currentWeapon.transform.position = currentWeaponPos.position;
            currentWeapon.transform.rotation = currentWeaponPos.rotation;

            //får vapnet att sluta rotera
            currentWeapon.isRotating = false;

        }
    }
}
