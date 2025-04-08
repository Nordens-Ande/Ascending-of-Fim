using NUnit.Framework;
using UnityEngine;

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
    bool isReadyToFire;
    bool isReloading;

    int rayLength;
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
        rayLength = 10000;
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
            Debug.Log("weapon data for enemy not retreived");
        }
    }

    void Shoot()
    {
        isReadyToFire = false;
        weapon.GetComponent<WeaponScript>().DecreaseBullets(1); // maybe change if shotgun?
        RaycastHit hit = shootScript.ShootRay();
        CheckRay(hit);
        Invoke("ResetIsReadyToFire", CalculateFireRate());
    }

    void Reload()
    {
        isReloading = true;
        Invoke("FinnishReload", weaponData.reloadTime);
    }

    void FinnishReload()
    {
        weapon.GetComponent<WeaponScript>().ReloadBullets();
        isReloading = false;
    }

    void ResetIsReadyToFire()
    {
        isReadyToFire = true;
    }

    float CalculateFireRate()
    {
        float fireRate = 60 / weaponData.fireRate;
        return fireRate;
    }

    void CheckRay(RaycastHit hit)
    {
        if (hit.collider != null)
        {
            if (hit.transform.CompareTag("Player"))
            {
                hit.transform.gameObject.GetComponent<PlayerHealth>().ApplyDamage(weaponData.damage);
            }
        }
    }

    private void Update()
    {
        if(weapon.GetComponent<WeaponScript>().bulletsLeft <= 0 && !isReloading)
        {
            Reload();
        }
        if (isShooting && isReadyToFire && !isReloading && weapon.GetComponent<WeaponScript>().bulletsLeft > 0)
        {
            Shoot();
        }
    }
}
