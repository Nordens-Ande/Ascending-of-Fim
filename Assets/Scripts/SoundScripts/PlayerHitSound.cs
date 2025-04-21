using UnityEngine;

public class PlayerHitSound : MonoBehaviour
{
    public AudioSource playerHitaudioSource;
    public AudioClip playerHurtSoundEffect;
    int health;
    private bool deadmanswitch;
    

    void Start()
    {
        playerHitaudioSource = GetComponent<AudioSource>();
        deadmanswitch = false;
        //health = GetComponent<PlayerHealth>().Health;
    }

    void Update()
    {
        IsPlayerHit();

    }

    void SoundActivate() 
    { 
        playerHitaudioSource.Play();
    }

    //Denna metod ger ett hurtsound vid varje fj�rdedel av livet som har tagit borts
    //Deadmansswitch anv�nds f�r att hela tiden g�ra s� att ljudet kan anv�ndas igen
    void IsPlayerHit() 
    {
        if (health < 100 && health > 75 && deadmanswitch == false) 
        { 
            SoundActivate(); 
            deadmanswitch=true;

        
        }
        if (health < 75 && health > 50 && deadmanswitch == true) 
        {
            SoundActivate();
            deadmanswitch=false;
        
        }
        if (health < 50 && health > 25 && deadmanswitch == false)
        {
            SoundActivate();
            deadmanswitch = true;

        }
        if (health < 25 && health > 0 && deadmanswitch == true)
        {
            SoundActivate();
            deadmanswitch = false;

        }

    }
}
