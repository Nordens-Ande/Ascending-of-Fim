using System.Collections;
using UnityEngine;

public interface IWeapon
{
    public WeaponData GetWeaponData();
    public void Equip();
    //public void Unequip();
}

public class WeaponScript : MonoBehaviour, IWeapon
{
    bool initialized = false;

    [SerializeField] WeaponData WeaponData;
    [SerializeField] private float WeaponRotationSpeed;

    [SerializeField] public Transform RightHand;
    [SerializeField] public Transform LeftHand;

    [SerializeField] private GameObject bulletTrailPrefab;
    [SerializeField] private Transform bulletOrigin;

    public int bulletsLeft { get; private set; }

    private Rigidbody weaponBody;

    public bool IsRotating { get; set; }

    Coroutine destroyCoroutine;// used to cancel and check if DestroyOnGround coroutine is running

    public void Start()
    {
        if(initialized) return;

        bulletsLeft = WeaponData.ammoCapacity;
        weaponBody = GetComponent<Rigidbody>();
        CheckIfWeaponBodyNull();
        IsRotating = true;

        if (weaponBody)
        {
            weaponBody.isKinematic = false;
        }
        initialized = true;
    }

    public void Initialized() // use instead of start
    {
        if(initialized) return;
        
        bulletsLeft = WeaponData.ammoCapacity;
        weaponBody = GetComponent<Rigidbody>();
        CheckIfWeaponBodyNull();
        IsRotating = true;

        if (weaponBody)
        {
            weaponBody.isKinematic = false;
        }
        initialized = true;
    }

    public void DecreaseBullets(int amount) //used when the player or enemy shoots the weapon, reduces amount of bullet by 1
    {
        bulletsLeft -= amount;
    }

    public void ReloadBullets() //reloads the weapons bullets
    {
        bulletsLeft = WeaponData.ammoCapacity;
    }

    public void Update()
    {
        if (IsRotating)
            transform.Rotate(Vector3.up * WeaponRotationSpeed * (1 - Mathf.Exp(-WeaponRotationSpeed * Time.deltaTime)));
    }

    public WeaponData GetWeaponData()
    {
        return WeaponData;
    }

    public void CheckIfWeaponBodyNull() // used to fix issue with enemy spawn nullreferences
    {
        if (weaponBody == null)
        {
            weaponBody = GetComponent<Rigidbody>();
        }
    }

    public void Equip() // weapon has been equipped, change weapon behaviour.
    {
        if(destroyCoroutine != null)
        {
            StopCoroutine(destroyCoroutine);
            destroyCoroutine = null;
        }
        if(GetComponent<Collider>())
        {
            GetComponent<Collider>().enabled = false;
        }
        CheckIfWeaponBodyNull();
        weaponBody.constraints = RigidbodyConstraints.None;
        weaponBody.isKinematic = true;
        IsRotating = false;
    }

    public void Unequip(bool enemyDropped) // enemyDropped = true if enemy is the one dropping weapon, a 80% chance that the weapon gets removed is then added
                                           // change behaviour of the weapon, the weapon will now spin on the ground
    {
        if(enemyDropped)
        {
            int value = Random.Range(1, 101);
            if (value > 20)
            {
                Destroy(gameObject);
            }
        }
        weaponBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        transform.rotation = Quaternion.identity;
        GetComponent<Collider>().enabled = true;
        weaponBody.isKinematic = false;
        weaponBody.linearVelocity = Vector3.zero;
        weaponBody.angularVelocity = Vector3.zero;
        IsRotating = true;
        destroyCoroutine = StartCoroutine(DestroyOnGround());
    }

    IEnumerator DestroyOnGround() // destory a weapon 8 seconds after it was dropped
    {
        yield return new WaitForSeconds(8);
        Destroy(gameObject);
    }

    public void SpawnBulletTrail(Vector3 endPoint) //spawn bullet trails
    {
        if (bulletTrailPrefab == null)// || bulletOrigin == null) 
            return;

        GameObject lineObj = Instantiate(bulletTrailPrefab);
        LineRenderer line = lineObj.GetComponent<LineRenderer>();

        Vector3 start = bulletOrigin.position;
        line.SetPosition(0, start);
        line.SetPosition(1, endPoint);

        Destroy(lineObj, 0.05f); // lower number for "faster" effect
    }
}
