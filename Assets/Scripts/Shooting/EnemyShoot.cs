using NUnit.Framework;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyShoot : MonoBehaviour
{
    [Header("Possible Guns")]
    [SerializeField] GameObject pistol;
    //[SerializeField] GameObject rifle;
    //[SerializeField] GameObject shotgun;

    [Space]

    [SerializeField] Shoot shootScript;

    //Decide what weapon enemy will have
    enum Weapon { pistol, rifle, shotgun }
    GameObject weapon;
    WeaponData weaponData;

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
        GameObject[] weapons = { pistol }; //rifle, shotgun };
        weapon = DecideWeapon(weapons);
        if(weapon != null )
        {
            RetrieveWeaponData();
        }
        isShooting = false;
        isReadyToFire = true;
        isReloading = false;
    }

    GameObject DecideWeapon(GameObject[] weapons)
    {
        int random = Random.Range(0, weapons.Length);
        Debug.Log(weapons[random].ToString());
        return weapons[random];
    }

    void RetrieveWeaponData()
    {
        weaponData = weapon.GetComponent<WeaponScript>().GetWeaponData();
        if( weaponData == null)
        {
            Debug.Log("no weapon data retreived for enemy");
        }
    }

    void Shoot()
    {
        isReadyToFire = false;
        weapon.GetComponent<WeaponScript>().DecreaseBullets(1); // maybe change if shotgun?
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
        weapon.GetComponent<WeaponScript>().ReloadBullets();
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
        if (weapon.GetComponent<WeaponScript>().bulletsLeft <= 0 && !isReloading)
        {
            Reload();
        }
        if (isShooting && isReadyToFire && !isReloading && weapon.GetComponent<WeaponScript>().bulletsLeft > 0)
        {
            Shoot();
        }
    }
}
