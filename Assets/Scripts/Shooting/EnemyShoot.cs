using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyShoot : MonoBehaviour
{
    [SerializeField] Shoot shootScript;

    WeaponData weaponData;//retrieve from enemyWeaponInventory script
    WeaponScript weaponScript;//retrieve from enemyWeaponInventory script

    //Shooting logic
    bool isReadyToFire = true;
    bool isReloading;

    bool isShooting; //EnemyAiController script will set to true if enemystate is shooting, and false when not, controlling when Shoot() get called in update
    public void IsShooting(bool f)
    {
        isShooting = f;
    }

    private void Start()
    {
        isShooting = false;
        isReadyToFire = true;
        isReloading = false;
    }

    public void SetWeaponData(WeaponData weaponData, WeaponScript weaponScript)
    {
        this.weaponData = weaponData;
        this.weaponScript = weaponScript;
        if(this.weaponData == null || this.weaponScript == null)
        {
            Debug.Log("weaponData or weaponScript = null for enemy");
        }
    }

    void Shoot()
    {
        isReadyToFire = false;
        weaponScript.DecreaseBullets(1); // maybe change if shotgun?
        RaycastHit hit = shootScript.ShootRay();
        CheckRay(hit);
        StartCoroutine(ResetIsReadyToFire());
    }

    void Reload()
    {
        isReloading = true;
        StartCoroutine(FinishReload(weaponData.reloadTime));
    }

    IEnumerator FinishReload(float reloadTime)
    {
        yield return new WaitForSeconds(reloadTime);
        weaponScript.ReloadBullets();
        isReloading = false;
    }

    IEnumerator ResetIsReadyToFire()
    {
        yield return new WaitForSeconds(CalculateFireRate());
        isReadyToFire = true;
    }

    float CalculateFireRate()
    {
        float fireRate = 60 / weaponData.fireRate;
        return fireRate;
    }

    void CheckRay(RaycastHit hit)
    {
        if (hit.collider == null)
        {
            return;
        }

        if (hit.transform.CompareTag("Player"))
        {
            PlayerHealth health = hit.transform.GetComponent<PlayerHealth>();
            if(health != null)
            {
                health.ApplyDamage(weaponData.damage);
            }
        }
    }

    private void Update()
    {
        if (weaponScript.bulletsLeft <= 0 && !isReloading)
        {
            Reload();
        }
        if (isShooting && isReadyToFire && !isReloading && weaponScript.bulletsLeft > 0)
        {
            Shoot();
        }
    }
}
