using System.Collections;
using UnityEngine;

public class ShieldScript : MonoBehaviour
{
    public GameObject owner { get; private set; } // used by the enemy or player when shooting to make the raycast ignore this shield collider :)

    [SerializeField] public Transform HandPos;
    Rigidbody rigidBody;

    bool isRotating;
    float rotationSpeed;

    int health;

    Coroutine destroyCoroutine;

    void Start()
    {
        health = 50;
        isRotating = true;
        rotationSpeed = 10;

        rigidBody = GetComponent<Rigidbody>();
        if(rigidBody != null)
        {
            rigidBody.isKinematic = false;
        }
    }

    public void CheckIfShieldBodyNull() // used to fix issue with enemy spawn nullreferences
    {
        if (rigidBody == null)
        {
            rigidBody = GetComponent<Rigidbody>();
        }
    }

    public void SetOwner(GameObject owner)
    {
        this.owner = owner;
    }

    public void Equip()
    {
        if (destroyCoroutine != null)
        {
            StopCoroutine(destroyCoroutine);
            destroyCoroutine = null;
        }
        rigidBody.constraints = RigidbodyConstraints.None;
        rigidBody.isKinematic = true;
        isRotating = false;
        gameObject.layer = LayerMask.NameToLayer("Shield");
    }

    public void Unequip(bool enemyDropped)
    {
        if (enemyDropped)
        {
            int value = Random.Range(1, 101);
            if (value > 100)
            {
                Destroy(gameObject);
            }
        }
        rigidBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        transform.rotation = Quaternion.identity;
        //collider
        rigidBody.isKinematic = false;
        rigidBody.linearVelocity = Vector3.zero;
        rigidBody.angularVelocity = Vector3.zero;
        isRotating = true;
        gameObject.layer = LayerMask.NameToLayer("ShieldIgnore");
        destroyCoroutine = StartCoroutine(DestroyOnGround());
    }

    IEnumerator DestroyOnGround()
    {
        yield return new WaitForSeconds(8);
        Destroy(gameObject);
    }

    public int GetHealth()
    {
        return health;
    }

    public void DecreaseHealth(int amount)
    {
        health -= amount;
    }

    void Update()
    {
        if (isRotating)
            transform.Rotate(Vector3.up * rotationSpeed * (1 - Mathf.Exp(-rotationSpeed * Time.deltaTime)));
    }
}
