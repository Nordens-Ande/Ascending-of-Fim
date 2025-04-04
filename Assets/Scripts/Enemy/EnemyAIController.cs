using System;
using UnityEngine;

public class EnemyAIController : MonoBehaviour
{
    [SerializeField] EnemyMove enemyMove;

    enum EnemyState {searching = 2, movingToPlayerLastKnown = 3, chasing = 4, shooting = 5, dead = 6 }
    EnemyState previousEnemyState;
    [SerializeField] EnemyState enemyState;

    GameObject player;

    const float shootingDistance = 4.5f;
    const int searchingDistance = 15;

    LayerMask layerMask;
    [SerializeField] bool lineOfSight;
    bool lineOfSightLastUpdate;
    Vector3 playerLastKnownPosition;
    bool movingToPlayerLastKnownPos;

    void Start()
    {
        layerMask = ~LayerMask.GetMask("Enemy");
        player = GameObject.Find("Player");
        previousEnemyState = EnemyState.searching;
        enemyState = EnemyState.searching;
        enemyMove.wandering = true;
        lineOfSight = false;
        lineOfSightLastUpdate = false;
        movingToPlayerLastKnownPos = false;
    }

    void DecideEnemyState()
    {
        float distanceToPlayer = CalculateDistanceToPlayer();

        if(lineOfSight && distanceToPlayer <= shootingDistance)
        {
            enemyState = EnemyState.shooting;

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
        if(enemyState == EnemyState.shooting)
        {
            enemyMove.wandering = false;
            enemyMove.StopMoving();
        }
        else if(enemyState == EnemyState.chasing)
        {
            enemyMove.wandering = false;
            enemyMove.StartMoving();
        }
        else if(enemyState == EnemyState.movingToPlayerLastKnown)
        {
            enemyMove.wandering = false;
            enemyMove.SetDestination(playerLastKnownPosition);
            enemyMove.StartMoving();
        }
        else if(enemyState == EnemyState.searching)
        {
            enemyMove.wandering = true;
        }
    }

    void ResetDestination()
    {
        if(!enemyMove.agent.isStopped && enemyState == EnemyState.chasing)
        {
            enemyMove.SetDestination(player.transform.position);
        }
    }

    void SetLastSpottedPos()
    {
        if(lineOfSightLastUpdate && !lineOfSight)
        {
            playerLastKnownPosition = player.transform.position; //maybe not correct (y-axis)
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
        if (Physics.Raycast(ray, out hit, searchingDistance, layerMask))
        {
            if (hit.transform != null && hit.transform.CompareTag("Player"))
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

    void Update()
    {
        lineOfSight = CheckForLineOfSight();
        
        ResetDestination();
        SetLastSpottedPos();
        CheckIfEnemyReachedPlayerLastKnown();
        DecideEnemyState();

        if(enemyState != previousEnemyState)
        {
            UpdateEnemyBehaviour();
        }

        lineOfSightLastUpdate = lineOfSight;
        previousEnemyState = enemyState;
    }
}
