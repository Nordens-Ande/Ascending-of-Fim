using UnityEngine;

public class EnemyAIController : MonoBehaviour
{
    [SerializeField] EnemyMove enemyMove;

    enum EnemyState { idle = 1, searching = 2, movingToPLK = 3, chasing = 4, shooting = 5, dead = 6 }
    EnemyState previousEnemyState;
    EnemyState enemyState;

    [SerializeField] GameObject player;

    bool lineOfSight;
    bool lineOfSightLastUpdate;
    Vector3 playerLastKnownPosition;
    bool movingTowardsPlayerLastKnownPos;

    void Start()
    {
        previousEnemyState = EnemyState.idle;
        enemyState = EnemyState.idle;
        lineOfSight = false;
        lineOfSightLastUpdate = false;
        movingTowardsPlayerLastKnownPos = false;
    }

    void DecideEnemyState()
    {
        float distanceToPlayer = CalculateDistanceToPlayer();
        //if(lineOfSight)
        //{
        //    movingTowardsPlayerLastKnownPos = false;
        //}

        if(lineOfSight && distanceToPlayer <= 15)
        {
            enemyState = EnemyState.shooting;
        }
        else if(lineOfSight && distanceToPlayer > 15)
        {
            enemyState = EnemyState.chasing;
        }
        else if(movingTowardsPlayerLastKnownPos == true)
        {
            enemyState = EnemyState.movingToPLK;
        }
        else if(!lineOfSight && distanceToPlayer <= 15)
        {
            enemyState = EnemyState.searching;
        }
        else
        {
            enemyState = EnemyState.idle;
        }
    }

    void UpdateEnemyBehaviour()
    {
        if(enemyState == EnemyState.shooting)
        {
            // stop moving?
            //shoot
        }
        else if(enemyState == EnemyState.chasing)
        {
            enemyMove.SetDestination(player.transform.position); //maybe not correct (y-axis)
            //move
        }
        else if(enemyState == EnemyState.movingToPLK)
        {
            enemyMove.SetDestination(playerLastKnownPosition);
            //move
        }
        else if(enemyState == EnemyState.searching)
        {
            //maybe set random pos and move there?
            //move
        }
        else if(enemyState == EnemyState.idle)
        {
            //enemyMove.stop
        }
    }

    void SetLastSpottedPos()
    {
        if(lineOfSightLastUpdate && !lineOfSight)
        {
            playerLastKnownPosition = player.transform.position; //maybe not correct (y-axis)
            movingTowardsPlayerLastKnownPos = true;
        }
    }

    void CheckIfEnemyReachedPLK()
    {
        if (transform.position == playerLastKnownPosition) //maybe not correct (y-axis)
        {
            movingTowardsPlayerLastKnownPos = false;
        }
    }

    bool CheckForLineOfSight()
    {
        Vector3 directionToPlayer = CalculateDirectionToPlayer();
        Ray ray = new Ray(transform.position, directionToPlayer);
        int rayLength = 100;
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, rayLength))
        {

        }

        if (hit.transform.tag == "Player") //?
        {
            movingTowardsPlayerLastKnownPos = false;
            return true;
        }
        else
        {
            return false;
        }
    }

    Vector3 CalculateDirectionToPlayer() //maybe not correct (y-axis)
    {
        return Vector3.Normalize(transform.position - player.transform.position);
    }

    float CalculateDistanceToPlayer()
    {
        return Vector3.Distance(transform.position, player.transform.position);//maybe not correct (y-axis)
    }

    void Update()
    {
        lineOfSight = CheckForLineOfSight();

        SetLastSpottedPos();
        CheckIfEnemyReachedPLK();
        DecideEnemyState();

        if(enemyState != previousEnemyState)
        {
            UpdateEnemyBehaviour();
        }

        lineOfSightLastUpdate = lineOfSight;
        previousEnemyState = enemyState;
    }
}
