using System.IO;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMove : MonoBehaviour
{
    public NavMeshAgent agent { get; private set; }

    public bool wandering { get; set; }
    bool movingToRandomPos;

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

    public void SetDestination(Vector3 destination)
    {
        agent.destination = destination;
    }

    public void StartMoving()
    {
        agent.isStopped = false;
    }

    public void StopMoving()
    {
        agent.isStopped = true;
    }

    void WanderController()
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

    Vector3 GetRandomPos(Vector3 enemyPos)
    {
        int maxDistance = 9;
        int minDistance = 2;
        for(int i = 0; i < 10; i++)
        {
            Vector3 randomDirection = Random.insideUnitSphere * maxDistance;
            randomDirection += enemyPos;
            NavMeshHit hit;
            if(NavMesh.SamplePosition(randomDirection, out hit, maxDistance, NavMesh.AllAreas))
            {
                if(Vector3.Distance(hit.normal, enemyPos) > minDistance)
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
