using System;
using Unity.Mathematics;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class EnemyAIController : MonoBehaviour
{
    [SerializeField] EnemyMove enemyMove;

    enum EnemyState { idle = 1, searching = 2, movingToPlayerLastKnown = 3, chasing = 4, shooting = 5, dead = 6 }
    EnemyState previousEnemyState;
    [SerializeField] EnemyState enemyState;
    [SerializeField] float RotationSpeed;
    [SerializeField] Transform OrientationObject;

    GameObject player;

    const float shootingDistance = 4.5f;
    const int searchingDistance = 15;

    LayerMask layerMask;
    bool lineOfSight;
    bool lineOfSightLastUpdate;
    Vector3 playerLastKnownPosition;
    bool movingToPlayerLastKnownPos;

    Vector3 lastPos;

    void Start()
    {
        layerMask = ~LayerMask.NameToLayer("Enemy");
        player = GameObject.Find("Player");
        previousEnemyState = EnemyState.idle;
        enemyState = EnemyState.idle;
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
        else if(!lineOfSight && distanceToPlayer <= searchingDistance)
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
            enemyMove.StopMoving();
        }
        else if(enemyState == EnemyState.chasing)
        {
            
            enemyMove.StartMoving();
        }
        else if(enemyState == EnemyState.movingToPlayerLastKnown)
        {
            enemyMove.SetDestination(playerLastKnownPosition);
            enemyMove.StartMoving();
        }
        else if(enemyState == EnemyState.searching)
        {
            enemyMove.StopMoving();
        }
        else if(enemyState == EnemyState.idle)
        {
            enemyMove.StopMoving();
        }
    }

    void ResetDestination()
    {
        if(!enemyMove.Agent.isStopped && enemyState != EnemyState.movingToPlayerLastKnown)
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

    void CheckIfEnemyReachedPLK()
    {
        if (transform.position == playerLastKnownPosition) //maybe not correct (y-axis)
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

    private float CalculateRotationToPlayer()
    {
        Vector3 directionToPlayer = player.transform.position - transform.position;
        float angle = Mathf.Atan2(directionToPlayer.x, directionToPlayer.z) * Mathf.Rad2Deg;

        return angle;
    }

    void Update()
    {
        lineOfSight = CheckForLineOfSight();
        float angle = CalculateRotationToPlayer();
        Vector3 moveVector = transform.position - lastPos;
        moveVector.y = 0;
        lastPos = transform.position;

        ResetDestination();
        SetLastSpottedPos();
        CheckIfEnemyReachedPLK();
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
