using System.IO;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMove : MonoBehaviour
{
    public NavMeshAgent agent { get; private set; }

    public bool wandering { get; set; } //used to decide if the enemy should wander which is an enemy state when they walk randomly on the map
    bool movingToRandomPos; // used to determine if the enemy has reached the position when wandering and a new position can be chosen after a timer

    float timer = 0;
    float waitTime = 1.5f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = 3;
        agent.angularSpeed = 3;
        movingToRandomPos = false;
    }

    public Vector3 GetAgentPosition()
    {
        return agent.transform.position;
    }

    public void SetDestination(Vector3 destination) // this method along with StartMoving and StopMoving is called from the
                                                    // EnemyAiController script to decide if the enemy should be moving or not and what position
    {
        agent.destination = destination;
    }

    public void StartMoving()
    {
        if(agent.isOnNavMesh)
            agent.isStopped = false;
    }

    public void StopMoving()
    {
        if(agent.isOnNavMesh)
            agent.isStopped = true;
    }

    void WanderController() //check if a random position should be chosen based on if the enemy is already moving to a position
    {
        if(wandering)
        {
            if (movingToRandomPos && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                movingToRandomPos = false;
                timer = waitTime;
            }
            if (!movingToRandomPos && timer <= 0)
            {
                MoveToRandomPos();
            }
            else if(!movingToRandomPos)
            {
                timer -= Time.deltaTime;
            }
        }
        
    }

    public void MoveToRandomPos()
    {
        if (movingToRandomPos) return;

        Vector3 randomPos = GetRandomPos(transform.position);

        if(randomPos != Vector3.zero)
        {
            agent.destination = randomPos;
            StartMoving();
            movingToRandomPos = true;
        }
    }

    Vector3 GetRandomPos(Vector3 enemyPos) //choses a random position on the navmesh in a random direction and within a radius.
    {
        int maxDistance = 8;
        int minDistance = 2;
        for(int i = 0; i < 10; i++)
        {
            Vector3 randomDirection = Random.insideUnitSphere * maxDistance;
            randomDirection += enemyPos;
            NavMeshHit hit;
            if(NavMesh.SamplePosition(randomDirection, out hit, 1, NavMesh.AllAreas))
            {
                if(Vector3.Distance(hit.position, enemyPos) > minDistance)
                {
                    return hit.position;
                }
            }
        }
        return Vector3.zero;
    }

    void Update()
    {
        WanderController();
    }
}
