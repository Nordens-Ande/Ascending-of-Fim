using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField] Shoot shootScript;
    [SerializeField] PlayerInventory playerInventory;

    WeaponData weaponData;
    
    bool isReadyToShoot = true;
    bool isReloading;

    void Start()
    {
        RetrieveWeaponData();
        isReadyToShoot = true;
        isReloading = false;
    }

    void RetrieveWeaponData()
    {
        weaponData = playerInventory.equipped.GetComponent<WeaponScript>().GetWeaponData();
    }

    void OnAttack(InputValue input)
    {
        if (isReadyToShoot && !isReloading && playerInventory.equipped != null && playerInventory.equipped.GetComponent<WeaponScript>().bulletsLeft > 0) 
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

    void OnReload(InputValue input)
    {
        if(playerInventory.equipped.GetComponent<WeaponScript>().bulletsLeft < weaponData.ammoCapacity && !isReloading)
        {
            isReloading = true;
            Invoke("FinishReload", weaponData.reloadTime);
        }
    }

    void FinishReload()
    {
        playerInventory.equipped.GetComponent<WeaponScript>().ReloadBullets();
        isReloading = false;
    }

    void Shoot()
    {
        isReadyToShoot = false;
        playerInventory.equipped.GetComponent<WeaponScript>().DecreaseBullets(1); // maybe change if shotgun?
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
