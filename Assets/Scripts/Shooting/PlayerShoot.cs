using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField] Shoot shootScript;
    [SerializeField] PlayerInventory playerInventory;

    WeaponData weaponData;

    bool isReadyToShoot;
    bool isReloading;

    void Start()
    {
        RetrieveWeaponData();
    }

    void RetrieveWeaponData()
    {
        weaponData = playerInventory.equipped.GetComponent<WeaponScript>().GetWeaponData();
    }

    void OnAttack(InputValue input)
    {
        if (isReadyToShoot && !isReloading && playerInventory.equipped != null) // check bullets left
        {
            Shoot();
        }
    }

    void ResetIsReadyToShoot()
    {
        isReadyToShoot = true;
    }

    float CalculateFireRate()
    {
        float fireRate = 60/weaponData.fireRate;
        return fireRate;
    }

    void Reload()
    {
        isReloading = true;
        Invoke("FinishReload", weaponData.reloadTime);
    }

    void FinishReload()
    {
        isReloading = false;
    }

    void Shoot()
    {
        isReadyToShoot = false;
        RaycastHit hit = shootScript.ShootRay();
        CheckRay(hit);
        Invoke("ResetIsReadyToShoot", CalculateFireRate());
    }

    void CheckRay(RaycastHit hit)
    {
        if(hit.collider != null)
        {
            if (hit.transform.CompareTag("Enemy"))
            {
                hit.transform.gameObject.GetComponent<EnemyHealth>().ApplyDamage(weaponData.damage);
            }
        }
    }

    void Update()
    {
        if (playerInventory.equipped != null)
        {
            RetrieveWeaponData();
        }
    }
}
