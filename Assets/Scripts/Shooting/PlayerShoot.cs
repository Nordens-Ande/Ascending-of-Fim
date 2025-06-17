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

    void RetrieveWeaponData() //get the weaponScript and weaponData from the equipped weapon.
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

    IEnumerator ResetIsReadyToShoot() //this enumerator decides when the cooldown after firing is done, based on firerate
    {
        yield return new WaitForSeconds(CalculateFireRate());
        isReadyToShoot = true;
    }

    float CalculateFireRate()//firerate in weaponData is based on bullets per minute so convert that to seconds to be able to use in the enumerator above
    {
        float fireRate = 60/weaponData.fireRate;
        return fireRate;
    }

    void OnReload(InputValue input)
    {
        if(weaponScript.bulletsLeft < weaponData.ammoCapacity && !isReloading) //make sure bullets left is not "full" for the weapon type, and that we are not already reloading
        {
            isReloading = true;
            SEP.ReloadSoundEffect();
            StartCoroutine(FinishReload());
           
        }
    }

    IEnumerator FinishReload() //"finish" the reload after the reloadTime variable in the scriptableobject for the equipped weapon is finished,
    {
        yield return new WaitForSeconds(weaponData.reloadTime);
        weaponScript.ReloadBullets();
        isReloading = false;
        isReadyToShoot = true;
        reloadMessageShown = false;
    }

    void Shoot() //shoot the weapon
    {
        isReadyToShoot = false; 
        weaponScript.DecreaseBullets(1); // decrease ammo

        List<RaycastHit> hits;
        if (weaponData.weaponName.ToLower() == "shotgun") // check if shotgun, shoot 8 rays instead
        {
            hits = shootScript.ShootRay(8);
            SEP.ShotgunShooting();
        }
        else
        {
            hits = shootScript.ShootRay(1);
            SEP.shooting();

        }
        

        if (hudHandler != null)
        {
            hudHandler.FimShootingShake();
        }

        CheckRay(hits);
        StartCoroutine(ResetIsReadyToShoot());// restart the fire rate cooldown
    }

    void CheckRay(List<RaycastHit> hits) // called from the Shoot method, in this method we check what was hit by the ray/rays in the Shoot method
    {
        foreach (RaycastHit hit in hits)
        {
            Vector3 endPoint;
            if (hit.collider != null)
                endPoint = hit.point;
            else
                endPoint = shootScript.transform.position + shootScript.transform.forward * 1000f;

            weaponScript.SpawnBulletTrail(endPoint); // create a bullet trail

            if (hit.collider == null) continue;
            if (hit.transform.CompareTag("Enemy"))
            {
                hit.transform.gameObject.GetComponent<EnemyHealth>().ApplyDamage(weaponData.damage); //if enemy hit apply damage
            }
            else if(hit.transform.CompareTag("Shield"))
            {
                hit.transform.gameObject.GetComponent<ShieldScript>().DecreaseHealth(weaponData.damage); 
            }

            //
            ExplodingBarrel barrel = hit.transform.GetComponent<ExplodingBarrel>();
            if (barrel != null)
            {
                barrel.TakeDamage();
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
        
        if (isShooting && isReadyToShoot && !isReloading && weaponScript.bulletsLeft > 0)
        {
            Shoot();
            
            if (!weaponData.allowButtonHold) // cannot hold mouse down and shoot continuosly with pistols and shotguns
            {
                isShooting = false;
                //SEP.getShooting();
            }
        }
        if (weaponScript.bulletsLeft == 0 && Input.GetMouseButtonDown(0))
        {
            SEP.GunClick();

        }

        if (weaponScript.bulletsLeft == 0 && !reloadMessageShown)
        {
            SEP.NeedToRealoadsound();
            reloadMessageShown = true;
            hudHandler.setAnnounchment("Reload with R", 3);
        }
    }
}
