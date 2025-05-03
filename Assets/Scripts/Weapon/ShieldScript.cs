using System.Collections;
using UnityEngine;

public class ShieldScript : MonoBehaviour
{
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

    public void Equip()
    {
        if (destroyCoroutine != null)
        {
            StopCoroutine(destroyCoroutine);
            destroyCoroutine = null;
        }
        if (GetComponent<Collider>())
        {
            GetComponent<Collider>().enabled = false;
        }
        rigidBody.constraints = RigidbodyConstraints.None;
        rigidBody.isKinematic = true;
        isRotating = false;
        //gameObject.layer = layer;
    }

    public void Unequip(bool enemyDropped)
    {
        if (enemyDropped)
        {
            int value = Random.Range(1, 101);
            if (value > 20)
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
