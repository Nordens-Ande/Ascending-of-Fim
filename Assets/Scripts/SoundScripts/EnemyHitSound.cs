using UnityEngine;

public class EnemyHitSound : MonoBehaviour
{
    public AudioSource enemyHitAudiosource;
    public AudioClip enemyHitAudioclip;
    int EnemyHurting;
    private bool deadmanswitch;

    void Start()
    {
        enemyHitAudiosource = GetComponent<AudioSource>();
        //isEnemyHurting = GetComponent<EnemyHealth>().Health;
        deadmanswitch = false;

    }
    void Update()
    {
        IsEnemyHit();
    }

    void EnemyHurtSound() 
    { 
        enemyHitAudiosource.Play();
        
    
    }

    //Denna metod ger ett hurtsound vid varje fj�rdedel av livet som har tagit borts
    //Deadmansswitch anv�nds f�r att hela tiden g�ra s� att ljudet kan anv�ndas igen
    void IsEnemyHit() 
    {
        if (EnemyHurting < 100 && EnemyHurting > 75 && deadmanswitch == false)
        {
            EnemyHurtSound();
            deadmanswitch = true;


        }
        if (EnemyHurting < 75 && EnemyHurting > 50 && deadmanswitch == true)
        {
            EnemyHurtSound();
            deadmanswitch = false;

        }
        if (EnemyHurting < 50 && EnemyHurting > 25 && deadmanswitch == false)
        {
            EnemyHurtSound();
            deadmanswitch = true;

        }
        if (EnemyHurting < 25 && EnemyHurting > 0 && deadmanswitch == true)
        {
            EnemyHurtSound();
            deadmanswitch = false;

        }



    }
}
