using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    //Gun stats
    public int damage;
    public float timeBetweenShooting;
    public float timeBetweenShots;
    public bool allowButtonHold;
    public float spread;
    public float range;
    public float reloadTime;
    public int magazineSize;
    public int bulletsPerTap;
    public int bulletsLeft;
    public int bulletsShot;

    //bools
    public bool shooting;
    public bool readyToShoot;
    public bool reloading;

    //Reference
    public Camera TopDownDShooterCam;
    public Transform attackPoint;
    public RaycastHit rayHit;
    public LayerMask whatIsEnemy;


    private void Awake()
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;
    }
    private void Update()
    {
        MyInput();
    }
    private void MyInput() 
    {   
        if (allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);

        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading) Reload();

        //Shoot
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = bulletsPerTap;
            Shoot();
        }
    }

    private void Shoot()
    {
        readyToShoot = false;

        //Raycast
        if (Physics.Raycast(TopDownDShooterCam.transform.position, TopDownDShooterCam.transform.forward, out rayHit, range, whatIsEnemy))
        {
            Debug.Log("We hit " + rayHit.transform.name);

            if (rayHit.collider.CompareTag("Enemy"))
            {
                rayHit.collider.GetComponent<Enemy>().TakeDamage(damage); //lägg in enemy scriptet här, så att den tar skada
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

    private void ResetShot()
    {
        readyToShoot = true;
    }

    private void Reload() 
    {
        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }

    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        reloading = false;
    }
}
