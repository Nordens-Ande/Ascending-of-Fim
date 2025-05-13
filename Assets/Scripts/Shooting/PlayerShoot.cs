using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField] Shoot shootScript;
    [SerializeField] EquipWeapon equipWeapon;

    [SerializeField] HUDHandler hudHandler;
    private bool reloadMessageShown = false;
    [SerializeField]SoundEffectsPlayer SEP;

    WeaponData weaponData;
    WeaponScript weaponScript;

    public bool isShooting;
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
        weaponScript = equipWeapon.currentWeaponObject.GetComponent<WeaponScript>();

        //basically uptpade funktionen, ui ammo kollas varje frame och skriver ut hur mycket ammo man har
        if (hudHandler != null)
        {
            hudHandler.setAmmo(weaponScript.bulletsLeft);
        }
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
        if(weaponScript.bulletsLeft < weaponData.ammoCapacity && !isReloading)
        {
            isReloading = true;
            StartCoroutine(FinishReload());
        }
    }

    IEnumerator FinishReload()
    {
        yield return new WaitForSeconds(weaponData.reloadTime);
        weaponScript.ReloadBullets();
        isReloading = false;
        isReadyToShoot = true;
        reloadMessageShown = false;
    }

    void Shoot()
    {
        isReadyToShoot = false;
        weaponScript.DecreaseBullets(1);

        List<RaycastHit> hits;
        if (weaponData.weaponName.ToLower() == "shotgun")
        {
            hits = shootScript.ShootRay(8);
        }
        else
        {
            hits = shootScript.ShootRay(1);


        }
        SEP.shooting();

        if (hudHandler != null)
        {
            hudHandler.FimShootingShake();
        }

        CheckRay(hits);
        StartCoroutine(ResetIsReadyToShoot());
    }

    void CheckRay(List<RaycastHit> hits)
    {
        foreach (RaycastHit hit in hits)
        {
            Vector3 endPoint;
            if (hit.collider != null)
                endPoint = hit.point;
            else
                endPoint = shootScript.transform.position + shootScript.transform.forward * 1000f;

            weaponScript.SpawnBulletTrail(endPoint);

            if (hit.collider == null) continue;
            if (hit.transform.CompareTag("Enemy"))
            {
                hit.transform.gameObject.GetComponent<EnemyHealth>().ApplyDamage(weaponData.damage);
            }
            else if(hit.transform.CompareTag("Shield"))
            {
                hit.transform.gameObject.GetComponent<ShieldScript>().DecreaseHealth(weaponData.damage);
                Debug.Log("player hit enemy shield");
            }

            //
            ExplodingBarrel barrel = hit.transform.GetComponent<ExplodingBarrel>();
            if (barrel != null)
            {
                barrel.TakeDamage();
            }

            //if (hit.transform.CompareTag("ExplodingBarrel"))  detta kanske är ett bättre sätt
            //{
            //    ExplodingBarrel barrel = hit.transform.GetComponent<ExplodingBarrel>();
            //    if (barrel != null)
            //    {
            //        barrel.TakeDamage();
            //    }
            //}
        }
    }

    void Update()
    {
        if (equipWeapon.currentWeaponObject == null)
        {
            return;
        }
        
        RetrieveWeaponData();
        
        if (isShooting && isReadyToShoot && !isReloading && weaponScript.bulletsLeft > 0)
        {
            Shoot();
            
            if (!weaponData.allowButtonHold)
            {
                isShooting = false;
                //SEP.getShooting();
            }
        }

        if (weaponScript.bulletsLeft == 0 && !reloadMessageShown)
        {
            reloadMessageShown = true;
            hudHandler.setAnnounchment("Reload with R", 3);
        }
    }
}
