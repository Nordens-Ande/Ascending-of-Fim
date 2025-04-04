using System.IO;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMove : MonoBehaviour
{
    NavMeshAgent agent;
    public NavMeshAgent Agent {  get { return agent; } }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = 3;
        agent.angularSpeed = 3;
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
