using UnityEngine;

public interface IWeapon
{
    public WeaponData GetWeaponData();
    public void Equip();
    //public void Unequip();
}

public class WeaponScript : MonoBehaviour, IWeapon
{
    [SerializeField] WeaponData WeaponData;
    [SerializeField] private float WeaponRotationSpeed;

    [SerializeField] public Transform RightHand;
    [SerializeField] public Transform LeftHand;

    public int bulletsLeft { get; private set; }

    private Rigidbody weaponBody;

    public bool IsRotating { get; set; }

    public void Start()
    {
        bulletsLeft = WeaponData.ammoCapacity;
        weaponBody = GetComponent<Rigidbody>();
        IsRotating = true;

        if (weaponBody)
        {
            weaponBody.isKinematic = false;
        }
    }

    public void DecreaseBullets(int amount)
    {
        bulletsLeft -= amount;
    }

    public void ReloadBullets()
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

    public void Equip()
    {
        GetComponent<Collider>().enabled = false;
        Debug.Log("happened 1");
        weaponBody.isKinematic = true;
        Debug.Log("happened 2");
        IsRotating = false;
        Debug.Log("happened 3");
    }

    public void Unequip()
    {
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        GetComponent<Collider>().enabled = true;
        weaponBody.isKinematic = false;
        IsRotating = true;
        transform.parent = null;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            Debug.Log("Weapon touched ground");
            if (weaponBody)
            {
                weaponBody.constraints = RigidbodyConstraints.FreezePosition;
                weaponBody.isKinematic = true;
                IsRotating = true;
            }
        }
    }
}
