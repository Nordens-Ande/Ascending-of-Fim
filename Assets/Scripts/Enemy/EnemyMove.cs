using System.IO;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMove : MonoBehaviour
{
    NavMeshAgent agent;
    Vector3 movementSpeed;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
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

    void Update()
    {
        
    }
}
