using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class EnemyAIController : MonoBehaviour
{
    [SerializeField] EnemyMove enemyMove;
    [SerializeField] EnemyShoot enemyShoot;
    [SerializeField] SoundEffectsEnemy soundEffectsEnemy;
    [SerializeField]EnemyVoicelines enemyVoicelines;

    enum EnemyState {searching = 1, movingToPlayerLastKnown = 2, chasing = 3, standingShooting = 4, runningShooting = 5 } //movingToPlayerLastKnown means that the enemy has seen the player but lost
                                                                                                                          //line of sight, and the enemy moves towards the position they last say the player at
    EnemyState previousEnemyState;
    [SerializeField] EnemyState enemyState;
    [SerializeField] float RotationSpeed;
    [SerializeField] Transform OrientationObject;

    GameObject player;

    float stoppingDistance;
    float shootingDistance = 3;

    LayerMask layerMask;
    [SerializeField] bool lineOfSight;
    bool lineOfSightLastUpdate;
    Vector3 playerLastKnownPosition;
    bool movingToPlayerLastKnownPos;

    Vector3 lastPos;

    bool forceUpdateBehaviour;

    void Start()
    {
        stoppingDistance = SetStoppingDistance();
        layerMask = ~(LayerMask.GetMask("Enemy") | LayerMask.GetMask("Weapon") | LayerMask.GetMask("EnemyIgnore") | LayerMask.GetMask("EnemyLimbs") | LayerMask.GetMask("Shield") | LayerMask.GetMask("ShieldIgnore") | LayerMask.GetMask("FurnitureJumpScare") | LayerMask.GetMask("Keycard"));
        player = GameObject.FindWithTag("Player");
        previousEnemyState = EnemyState.searching;
        enemyState = EnemyState.searching;
        enemyMove.wandering = true;
        lineOfSight = false;
        lineOfSightLastUpdate = false;
        movingToPlayerLastKnownPos = false;
        forceUpdateBehaviour = true;
    }

    float SetStoppingDistance() //set a random distance from the player the enemy will stop to create more variation in enemy movement/behaviour
    {
        float random = Random.Range(2.5f, 3);
        return random;
    }

    void DecideEnemyState() //called every update to check if enemy behaviour needs to be changed based on certain factors
    {
        float distanceToPlayer = CalculateDistanceToPlayer();

        if(lineOfSight && distanceToPlayer <= stoppingDistance)
        {
            enemyState = EnemyState.standingShooting;
        }
        else if(lineOfSight && distanceToPlayer < shootingDistance)
        {
            enemyState = EnemyState.runningShooting;
        }
        else if(lineOfSight && distanceToPlayer > shootingDistance)
        {
            enemyState = EnemyState.chasing;
        }
        else if(movingToPlayerLastKnownPos == true)
        {
            enemyState = EnemyState.movingToPlayerLastKnown;
        }
        else
        {
            enemyState = EnemyState.searching;
        }
    }

    void UpdateEnemyBehaviour() //if the method aboves changes the behaviour an enemy should have,
                                //this method is called and ensures that the enemy behaves that way by changing turning on/off shooting etc
    {
        if(enemyState == EnemyState.standingShooting)
        {

            enemyMove.wandering = false;
            enemyShoot.IsShooting(true);
            //soundEffectsEnemy.SetIsShooting(true);
            enemyMove.StopMoving();

            enemyVoicelines.SetEnemyVoicelines(3);
        }
        else if(enemyState == EnemyState.runningShooting)
        {

            enemyMove.wandering = false;
            enemyShoot.IsShooting(true);
            //soundEffectsEnemy.SetIsShooting(true);
            enemyMove.StartMoving();

            enemyVoicelines.SetEnemyVoicelines(3);
        }
        else if(enemyState == EnemyState.chasing)
        {
 
            enemyMove.wandering = false;
            enemyShoot.IsShooting(false);
            //soundEffectsEnemy.SetIsShooting(false);
            enemyMove.StartMoving();

            enemyVoicelines.SetEnemyVoicelines(3);
        }
        else if(enemyState == EnemyState.movingToPlayerLastKnown)
        {

            enemyMove.wandering = false;
            enemyShoot.IsShooting(false);
            //soundEffectsEnemy.SetIsShooting(false);
            enemyMove.SetDestination(playerLastKnownPosition);
            enemyMove.StartMoving();

            enemyVoicelines.SetEnemyVoicelines(2);
        }
        else if(enemyState == EnemyState.searching)
        {
            enemyShoot.IsShooting(false);
            //soundEffectsEnemy.SetIsShooting(false);
            enemyMove.wandering = true;

            enemyVoicelines.SetEnemyVoicelines(1);
        }
    }

    void ResetDestination() //continuosly update the target positon for the NavMesh agent while following/chasing the player
    {
        if(!enemyMove.agent.isStopped && enemyState == EnemyState.chasing || enemyState == EnemyState.runningShooting)
        {
            enemyMove.SetDestination(player.transform.position);
        }
    }

    void SetLastSpottedPos() //checks if the enemy had line of sight and then lost it, updates enemy behaviour if true
    {
        if(lineOfSightLastUpdate && !lineOfSight)
        {
            playerLastKnownPosition = player.transform.position;
            movingToPlayerLastKnownPos = true;//this line updates the enemy behaviour
        }
    }

    void CheckIfEnemyReachedPlayerLastKnown() // checks if the enemy has reached the position they last saw the player, if so set the bool to false and enemy can behave differently again.
    {
        if (Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z),
                             new Vector3(playerLastKnownPosition.x, 0, playerLastKnownPosition.z)) < 0.2f)// need a bit of a buffer as the navmesh agent can stop a bit before or after the target position
        {
            movingToPlayerLastKnownPos = false;
        }
    }
    
    bool CheckForLineOfSight() // enemy checks for line of sight to the player, this is a condition for how the enemy will behave and is checked once per update
    {
        Vector3 directionToPlayer = CalculateDirectionToPlayer();
        Ray ray = new Ray(transform.position, directionToPlayer);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))//layermask sorts out certain layers such as, other enemies, weapons and shields.
        {
            if (hit.transform.CompareTag("Player"))
            {
                movingToPlayerLastKnownPos = false;
                return true;
            }
        }

        return false;
    }

    Vector3 CalculateDirectionToPlayer() //this method is used when checking line of sight to determine the direction of the raycast
    {
        return Vector3.Normalize(player.transform.position - transform.position);
    }

    float CalculateDistanceToPlayer() // also used to determine enemy behaviour, called every update in the DecideEnemyState method.
    {
        return Vector3.Distance(transform.position, player.transform.position);
    }

    private float CalculateRotationToPlayer() //rotation to player, used when the enemy is running towards/shooting at the player
    {
        Vector3 directionToPlayer = player.transform.position - transform.position;
        float angle = Mathf.Atan2(directionToPlayer.x, directionToPlayer.z) * Mathf.Rad2Deg;

        return angle;
    }

    void Update()
    {
        if(forceUpdateBehaviour) // used to call the updateEnemyBehaviour method once at the first update,
                                 // ensuring that the enemy behaves correctly even if no EnemyState changes have occured
        {
            UpdateEnemyBehaviour();
            forceUpdateBehaviour = false;
        }
        layerMask = ~(LayerMask.GetMask("Enemy") | LayerMask.GetMask("Weapon") | LayerMask.GetMask("EnemyIgnore") | LayerMask.GetMask("EnemyLimbs") | LayerMask.GetMask("Shield") | LayerMask.GetMask("ShieldIgnore") | LayerMask.GetMask("FurnitureJumpScare") | LayerMask.GetMask("Keycard"));

        lineOfSight = CheckForLineOfSight();
        float angle = CalculateRotationToPlayer();
        Vector3 moveVector = transform.position - lastPos;
        moveVector.y = 0;
        lastPos = transform.position;

        ResetDestination();
        SetLastSpottedPos();
        CheckIfEnemyReachedPlayerLastKnown();
        DecideEnemyState();
        if (lineOfSight && OrientationObject.rotation != Quaternion.Euler(0, angle, 0)) // rotate the enmy in the direction its moving
            OrientationObject.rotation = Quaternion.Lerp(OrientationObject.rotation, Quaternion.Euler(0, angle, 0), Time.deltaTime * RotationSpeed);
        else if (moveVector.magnitude > 0f)
            OrientationObject.rotation = Quaternion.Lerp(OrientationObject.rotation, Quaternion.Euler(0, Mathf.Atan2(moveVector.x, moveVector.z) * Mathf.Rad2Deg, 0), Time.deltaTime * RotationSpeed);

        if (enemyState != previousEnemyState)
        {
            UpdateEnemyBehaviour();
        }

        lineOfSightLastUpdate = lineOfSight;
        previousEnemyState = enemyState;
    }
}
