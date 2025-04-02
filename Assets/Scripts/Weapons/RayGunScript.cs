using UnityEngine;

public class RayGunScript : MonoBehaviour
{
    //För vapnet att skjuta,borde kopplas med shooting scriptet

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

    //weapon body
    private Rigidbody weaponBody;
    [SerializeField] private float WeaponRotationSpeed;

    public bool isRotating { get; set; }

    private void Start()
    {
        weaponBody = GetComponent<Rigidbody>();

        if (weaponBody) 
        {
            weaponBody.isKinematic = true;
        }

        isRotating = true;
    }
    private void Awake() //så man inte behver ladda om vapnet varje gång man starta spelet
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;
    }
    private void Update()
    {
        MyInput();

        if (isRotating) return;

            transform.Rotate(Vector3.up * WeaponRotationSpeed * (1- Mathf.Exp(-WeaponRotationSpeed * Time.deltaTime)));
    }
    private void MyInput() //för att skjuta och ladda om
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

    private void Shoot() //för att skjuta
    {
        readyToShoot = false;

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
        readyToShoot = true;
    }

    private void Reload() //för att ladda om
    {
        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }

    private void ReloadFinished() //för att sluta ladda om
    {
        bulletsLeft = magazineSize;
        reloading = false;
    }
}
