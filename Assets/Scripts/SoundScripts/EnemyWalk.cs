using UnityEngine;

public class EnemyWalk : MonoBehaviour
{
    public AudioSource enemyWalkSound;
    public AudioClip enemyWalkingClip;
    bool enemyIsWalking;
    bool deadMansSwitch;

    void Start()
    {
        enemyWalkSound = GetComponent<AudioSource>();
        deadMansSwitch = false;
        //enemyIsMoving = GetComponent<?>().namnetP�BoolenIEnemyscript;


    }

    void Update()
    { 
        playSoundOnWalk();
        //EnemyIsMoving = GetComponent<SoundEffectsPlayer>().namnetP�BoolenIEnemyscript;

    }
    //Deadmanswitch triggar loopen av ljudet 
    //Sedan om enemy slutar skjuta s� �terst�lls deadmanswitc
    //Och metoden kan s�ttas ig�ng igen
    public void playSoundOnWalk()
    {
        if (enemyIsWalking == true && deadMansSwitch == false)
        {
            walking();
            deadMansSwitch=true;
        }
        if(!enemyIsWalking)
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

