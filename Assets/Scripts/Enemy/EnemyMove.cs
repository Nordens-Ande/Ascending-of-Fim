using UnityEngine;
using UnityEngine.AI;

public class EnemyMove : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent;

    void Start()
    {
       
    }

    public Vector3 GetAgentPosition()
    {
        return agent.transform.position;
    }

    public void SetDestination(Vector3 destination)
    {
        agent.SetDestination(destination);
    }

    void Update()
    {
        
    }
}
