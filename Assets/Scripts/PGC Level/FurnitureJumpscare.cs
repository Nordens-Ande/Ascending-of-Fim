using System.Collections.Generic;
using UnityEngine;

public class FurnitureJumpscare : MonoBehaviour
{
    enum ActivationType
    {
        Enter,
        //Delayed,
        Distance,
        Exit
    }

    [Header("Activation")]
    [SerializeField] private bool activated = false;
    [SerializeField] private ActivationType activationType;
    [SerializeField] private float distanceToActivate;

    [Header("Objects")]
    [SerializeField] private EnemyAIController enemyToSpawn;
    [SerializeField] private List<Transform> spawnPositions = new List<Transform>();
    [SerializeField] private List<GameObject> objectsToDelete = new List<GameObject>();

    private BoxCollider hitboxCollider;
    private Furniture furniture;


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
        furniture = gameObject.GetComponent<Furniture>();
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

    public void ActivateJumpscare()
    {
        if (activated)
            return;

        activated = true;
        foreach (Transform trans in spawnPositions)
        {
            GameObject newEnemy = Instantiate(enemyToSpawn.gameObject);
            newEnemy.transform.position = trans.position;
            newEnemy.transform.rotation = trans.rotation;
        }
        foreach (GameObject gameObject in objectsToDelete)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (activationType != ActivationType.Enter)
            return;

        if (other.CompareTag("PlayerHitbox"))
        {
            Debug.Log("Player entered jumpscare hitbox");

            ActivateJumpscare();
        }
    }

    //DistanceCheck måste ske här/alternativt att ha en bool som låser upp en loop i update.
    private void OnTriggerStay(Collider other)
    {
        if (activationType != ActivationType.Distance)
            return;

        if (other.CompareTag("PlayerHitbox"))
        {
            Vector2 furnPos = new Vector2(transform.position.x + furniture.size.x / 2f, transform.position.z + furniture.size.y / 2f);
            Vector2 playerPos = new Vector2(other.transform.position.x, other.transform.position.z);

            if (Vector2.Distance(furnPos, playerPos) < distanceToActivate)
            {
                Debug.Log("Player is inside minimum zone of jumpscare");

                ActivateJumpscare();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (activationType != ActivationType.Exit)
            return;

        if (other.CompareTag("PlayerHitbox"))
        {
            Debug.Log("Player exited jumpscare hitbox");
            
            ActivateJumpscare();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
