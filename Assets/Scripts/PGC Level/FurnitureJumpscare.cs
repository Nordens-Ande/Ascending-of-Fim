using System.Collections.Generic;
using UnityEngine;

public class FurnitureJumpscare : MonoBehaviour
{
    [SerializeField] private EnemyAIController enemyToSpawn;
    [SerializeField] private List<Transform> spawnPositions = new List<Transform>();
    [SerializeField] private List<GameObject> objectsToDelete = new List<GameObject>();

    private BoxCollider hitboxCollider;

    void CreateCollider()
    {
        if (hitboxCollider == null)
        {
            hitboxCollider = gameObject.AddComponent<BoxCollider>(); //Or use another collider type
            //hitboxCollider = gameObject.GetComponent<BoxCollider>();
            hitboxCollider.isTrigger = true; //Set it as a trigger
        }
    }

    void Start()
    {
        CreateCollider();
    }

    public void UpdateCollider(Vector3 position, Vector3 size, bool isCentered = false)
    {
        if (hitboxCollider == null)
        {
            Debug.Log("No hitbox?");
            CreateCollider();
        }
        hitboxCollider.center = isCentered ? position : position + size / 2f;
        hitboxCollider.size = size;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerHitbox"))
        {
            Debug.Log("Player entered hitbox");
            //Activate your script or logic when the player enters
            //ActivateScript();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("PlayerHitbox"))
        {
            Debug.Log("Player is inside hitbox");
            //Continuously check or activate logic as long as the player stays inside
            //ActivateScript(); //Optional for continuous check
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlayerHitbox"))
        {
            Debug.Log("Player exited hitbox");
            //Deactivate or stop the logic when the player leaves
            //DeactivateScript();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
