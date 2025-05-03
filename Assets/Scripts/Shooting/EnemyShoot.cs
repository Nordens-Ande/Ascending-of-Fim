using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyShoot : MonoBehaviour
{
    [SerializeField] Shoot shootScript;
    GameObject player;

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
        player = GameObject.FindWithTag("Player");
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
        yield return new WaitForSeconds(CalculateFireRate() * 2);
        isReadyToFire = true;
    }

    float CalculateFireRate()
    {
        float fireRate = 60 / weaponData.fireRate;
        return fireRate;
    }

    void CheckRay(List<RaycastHit> hits)
    {
        foreach(RaycastHit hit in hits)
        {
            Vector3 endPoint;
            if (hit.collider != null)
                endPoint = hit.point;
            else
                endPoint = shootScript.transform.position + shootScript.transform.forward * 1000f;

            weaponScript.SpawnBulletTrail(endPoint);

            if (hit.collider == null) continue;
            if (hit.transform.CompareTag("Player"))
            {
                PlayerHealth health = hit.transform.GetComponent<PlayerHealth>();
                if (health != null)
                {
                    health.ApplyDamage(weaponData.damage);
                }
            }
            else if (hit.transform.CompareTag("Shield"))
            {
                ShieldScript playerShield = hit.transform.GetComponent<ShieldScript>();
                if(playerShield != null && playerShield.owner == player)
                {
                    playerShield.DecreaseHealth(weaponData.damage / 4);
                }
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
