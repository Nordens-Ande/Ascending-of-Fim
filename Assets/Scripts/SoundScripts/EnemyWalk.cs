using UnityEngine;
using UnityEngine.AI;

public class EnemyWalk : MonoBehaviour
{
    public AudioSource enemyWalkSound;
    public AudioClip enemyWalkingClip;
    bool deadMansSwitch;
    private NavMeshAgent agent;

    void Start()
    {
        enemyWalkSound = GetComponent<AudioSource>();
        deadMansSwitch = false;
        agent = GetComponent<NavMeshAgent>();


    }

    void Update()
    { 
        playSoundOnWalk();

    }
    //Deadmanswitch triggar loopen av ljudet 
    //Sedan om enemy slutar skjuta så återställs deadmanswitc
    //Och metoden kan sättas igång igen
    public void playSoundOnWalk()
    {
        if (agent.isStopped == false && deadMansSwitch == false)
        {
            walking();
            deadMansSwitch=true;
        }
        if(agent.isStopped == true)
        { 
            enemyWalkSound.Stop();
            deadMansSwitch=false;
        
        }
    }
    public void walking()
    {
        enemyWalkSound.clip = enemyWalkingClip;
        enemyWalkSound.Play();
    }

}

