using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class RayGunScript : MonoBehaviour
{
    //För vapnet att skjuta,borde kopplas med shooting scriptet

    [Header("GunStats")]
    //[SerializeField] public int damage;
    [SerializeField] public float timeBetweenShooting;
    [SerializeField] public float timeBetweenShots;
    [SerializeField] public bool allowButtonHold;
    [SerializeField] public float spread;
    [SerializeField] public float range;
    [SerializeField] public float reloadTime;
    [SerializeField] public int magazineSize;
    [SerializeField] public int bulletsPerTap;
    public int bulletsLeft;
    public int bulletsShot;
    public BulletController bullet;
    public Transform firePoint;

    //bools
    public bool isshooting;
    public bool isreadyToShoot;
    public bool isreloading;
    public bool isFiring;
    //Reference
    public Camera TopDownDShooterCam;
    public Transform attackPoint;
    public RaycastHit rayHit;
    public LayerMask whatIsEnemy;
    public EquipWeapon equipWeapon;
    //weapon body
    private Rigidbody weaponBody;
    [SerializeField] private float WeaponRotationSpeed;

 

    public bool IsRotating { get; set; }

    private void Start()
    {
        Debug.Log("I'm Here");

        weaponBody = GetComponent<Rigidbody>();
        

        if (weaponBody)
        {
            weaponBody.isKinematic = true;
            
        }

        IsRotating = true;
    }
    private void Awake() //så man inte behver ladda om vapnet varje gång man starta spelet
    {
        bulletsLeft = magazineSize;
        isreadyToShoot = true;
    }
    private void Update()
    {
        Debug.Log("I'm Here");
        MyInput();

        if (!IsRotating) return;

            transform.Rotate(Vector3.up * WeaponRotationSpeed * (1- Mathf.Exp(-WeaponRotationSpeed * Time.deltaTime)));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            if (weaponBody)
            {
                weaponBody.constraints = RigidbodyConstraints.FreezePosition;

                IsRotating = true;
            }
    }   }
    public void MyInput() //för att skjuta och ladda om
    {
        
        
        if (allowButtonHold) isshooting = Input.GetKey(KeyCode.Mouse0);
      
        else isshooting = Input.GetKeyDown(KeyCode.Mouse0);

        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !isreloading) Reload();
        
        //Shoot
        if (isreadyToShoot && isshooting && !isreloading && bulletsLeft > 0)
        {
         

            bulletsShot = bulletsPerTap;
            BulletController newBullet = Instantiate(bullet, firePoint.position, firePoint.rotation) as BulletController;
            Shoot();
            
        }
    }

    private void Shoot() //för att skjuta
    {
        isreadyToShoot = false;
        Debug.Log("Shooting");
        //Raycast
        if (Physics.Raycast(TopDownDShooterCam.transform.position, TopDownDShooterCam.transform.forward, out rayHit, range, whatIsEnemy))
        {
            Debug.Log("We hit " + rayHit.transform.name);

            if (rayHit.collider.CompareTag("Enemy"))
            {
                //rayHit.collider.GetComponent<Enemy>().TakeDamage(damage); //lägg in enemy scriptet här, så att den tar skada
            }
        }

        bulletsLeft--;
        bulletsShot--;
        Invoke("ResetShot", timeBetweenShooting);

        if (bulletsShot > 0 && bulletsLeft > 0) 
        {
            Invoke("Shoot", timeBetweenShots);
        }
    }

    private void ResetShot() //för att kunna skjuta igen
    {
        isreadyToShoot = true;
    }

    private void Reload() //för att ladda om
    {
        isreloading = true;
        Invoke("ReloadFinished", reloadTime);
    }

    private void ReloadFinished() //för att sluta ladda om
    {
        bulletsLeft = magazineSize;
        isreloading = false;
    }

    public void ChangeWeaponBehavior() 
    {
        if (weaponBody)
        {
            weaponBody.isKinematic = true;
            weaponBody.constraints = RigidbodyConstraints.None;

        }
    }
}
