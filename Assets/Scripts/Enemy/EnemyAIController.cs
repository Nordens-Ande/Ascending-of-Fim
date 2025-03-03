using UnityEngine;

public class EnemyAIController : MonoBehaviour
{
    enum EnemyState { idle = 1, searching = 2, movingToPLK = 3, chasing = 4, shooting = 5, dead = 6 }
    EnemyState enemyState;

    [SerializeField] GameObject player;

    bool lineOfSight;
    bool lineOfSightLastUpdate;
    Vector3 playerLastKnownPosition;
    bool movingTowardsPlayerLastKnownPos;

    void Start()
    {
        enemyState = EnemyState.idle;
        lineOfSight = false;
        lineOfSightLastUpdate = false;
        movingTowardsPlayerLastKnownPos = false;
    }

    void DecideEnemyState()
    {
        float distanceToPlayer = CalculateDistanceToPlayer();

        if(lineOfSight && distanceToPlayer <= 15)
        {
            enemyState = EnemyState.shooting;
            movingTowardsPlayerLastKnownPos = false;
        }
        else if(lineOfSight && distanceToPlayer > 15)
        {
            enemyState = EnemyState.chasing;
            movingTowardsPlayerLastKnownPos = false;
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

    void SetLastSpottedPos()
    {
        if(lineOfSightLastUpdate && !lineOfSight)
        {
            playerLastKnownPosition = player.transform.position;
            movingTowardsPlayerLastKnownPos = true;
        }
    }

    void CheckIfEnemyReachedPLK()
    {
        if (transform.position == playerLastKnownPosition)
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

        if (hit.transform.tag == "Player")
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    Vector3 CalculateDirectionToPlayer()
    {
        return Vector3.Normalize(transform.position - player.transform.position);
    }

    float CalculateDistanceToPlayer()
    {
        return Vector3.Distance(transform.position, player.transform.position);
    }

    void Update()
    {
        lineOfSight = CheckForLineOfSight();
        SetLastSpottedPos();
        CheckIfEnemyReachedPLK();
        DecideEnemyState();
        lineOfSightLastUpdate = lineOfSight;
    }
}
