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

    bool isShooting;
    bool isReadyToShoot = true;
    bool isReloading;

    void Start()
    {
        isShooting = false;
        isReadyToShoot = true;
        isReloading = false;
    }

    void RetrieveWeaponData()
    {
        weaponData = equipWeapon.currentWeaponObject.GetComponent<WeaponScript>().GetWeaponData();
    }

    void OnAttack(InputValue input)
    {
        isShooting = true;
    }

    void OnAttackStop(InputValue input)
    {
        isShooting = false;
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
        equipWeapon.currentWeaponObject.GetComponent<WeaponScript>().DecreaseBullets(1);

        List<RaycastHit> hits;
        if (weaponData.weaponName.ToLower() == "shotgun")
        {
            hits = shootScript.ShootRay(8);
        }
        else
        {
            hits = shootScript.ShootRay(1);
        }
        
        CheckRay(hits);
        StartCoroutine(ResetIsReadyToShoot());
    }

    void CheckRay(List<RaycastHit> hits)
    {
        foreach(RaycastHit hit in hits)
        {
            if (hit.collider == null) continue;
            if (hit.transform.CompareTag("Enemy"))
            {
                hit.transform.gameObject.GetComponent<EnemyHealth>().ApplyDamage(weaponData.damage);
            }
        }
    }

    void Update()
    {
        if (equipWeapon.currentWeaponObject == null)
        {
            return;
        }
        
        RetrieveWeaponData();
        
        if (isShooting && isReadyToShoot && !isReloading && equipWeapon.currentWeaponObject.GetComponent<WeaponScript>().bulletsLeft > 0)
        {
            Shoot();
            if(!weaponData.allowButtonHold)
            {
                isShooting = false;
            }
        }
    }
}
