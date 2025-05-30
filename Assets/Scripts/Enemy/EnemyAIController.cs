using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class EnemyAIController : MonoBehaviour
{
    [SerializeField] EnemyMove enemyMove;
    [SerializeField] EnemyShoot enemyShoot;
    [SerializeField] SoundEffectsEnemy soundEffectsEnemy;
    [SerializeField]EnemyVoicelines enemyVoicelines;

    enum EnemyState {searching = 1, movingToPlayerLastKnown = 2, chasing = 3, standingShooting = 4, runningShooting = 5 }
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

    float SetStoppingDistance()
    {
        float random = Random.Range(2.5f, 3);
        return random;
    }

    void DecideEnemyState()
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

    void UpdateEnemyBehaviour()
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

    void ResetDestination()
    {
        if(!enemyMove.agent.isStopped && enemyState == EnemyState.chasing || enemyState == EnemyState.runningShooting)
        {
            enemyMove.SetDestination(player.transform.position);
        }
    }

    void SetLastSpottedPos()
    {
        if(lineOfSightLastUpdate && !lineOfSight)
        {
            playerLastKnownPosition = player.transform.position;
            movingToPlayerLastKnownPos = true;
        }
    }

    void CheckIfEnemyReachedPlayerLastKnown()
    {
        if (Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z),
                             new Vector3(playerLastKnownPosition.x, 0, playerLastKnownPosition.z)) < 0.2f)
        {
            movingToPlayerLastKnownPos = false;
        }
    }
    
    bool CheckForLineOfSight()
    {
        Vector3 directionToPlayer = CalculateDirectionToPlayer();
        Ray ray = new Ray(transform.position, directionToPlayer);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            if (hit.transform.CompareTag("Player"))
            {
                movingToPlayerLastKnownPos = false;
                return true;
            }
        }

        return false;
    }

    Vector3 CalculateDirectionToPlayer() //maybe not correct (y-axis)
    {
        return Vector3.Normalize(player.transform.position - transform.position);
    }

    float CalculateDistanceToPlayer()
    {
        return Vector3.Distance(transform.position, player.transform.position);//maybe not correct (y-axis)
    }

    private float CalculateRotationToPlayer()
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
        if (lineOfSight && OrientationObject.rotation != Quaternion.Euler(0, angle, 0))
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
