using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField] Shoot shootScript;
    [SerializeField] EquipWeapon equipWeapon;

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
        weaponData = equipWeapon.currentWeaponObject.GetComponent<WeaponScript>().GetWeaponData();
    }

    void OnAttack(InputValue input)
    {
        if (isReadyToShoot && !isReloading && equipWeapon.currentWeaponObject != null && equipWeapon.currentWeaponObject.GetComponent<WeaponScript>().bulletsLeft > 0) 
        {
            Shoot();
        }
    }

    IEnumerator ResetIsReadyToShoot()
    {
        yield return new WaitForSeconds(CalculateFireRate());
        isReadyToShoot = true;
    }

    float CalculateFireRate()
    {
        float fireRate = 60/weaponData.fireRate;
        return fireRate;
    }

    void OnReload(InputValue input)
    {
        if(equipWeapon.currentWeaponObject.GetComponent<WeaponScript>().bulletsLeft < weaponData.ammoCapacity && !isReloading)
        {
            isReloading = true;
            StartCoroutine(FinishReload());
        }
    }

    IEnumerator FinishReload()
    {
        yield return new WaitForSeconds(weaponData.reloadTime);
        equipWeapon.currentWeaponObject.GetComponent<WeaponScript>().ReloadBullets();
        isReloading = false;
    }

    void Shoot()
    {
        isReadyToShoot = false;
        equipWeapon.currentWeaponObject.GetComponent<WeaponScript>().DecreaseBullets(1); // maybe change if shotgun?
        RaycastHit hit = shootScript.ShootRay();
        CheckRay(hit);
        StartCoroutine(ResetIsReadyToShoot());
    }

    void CheckRay(RaycastHit hit)
    {
        if(hit.collider == null)
        {
            return;
        }

        if (hit.transform.CompareTag("Enemy"))
        {
            hit.transform.gameObject.GetComponent<EnemyHealth>().ApplyDamage(weaponData.damage);
        }
    }

    void Update()
    {
        if (equipWeapon.currentWeaponObject != null)
        {
            RetrieveWeaponData();
        }
    }
}
