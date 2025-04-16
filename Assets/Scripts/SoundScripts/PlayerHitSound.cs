using UnityEngine;

public class PlayerHitSound : MonoBehaviour
{
    public AudioSource playerHitaudioSource;
    public AudioClip playerHurtSoundEffect;
    bool ishit;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerHitaudioSource = GetComponent<AudioSource>();
        //ishit = GetComponent<Script,d�r,ray,finns,n�r,Fim,blir,tr�ffad>().Namnet,p�,bool,som,anv�nds,n�r,ray,tr�ffar;
    }

    // Update is called once per frame
    void Update()
    {
        IsPlayerHit();

    }

    void SoundActivate() 
    { 
        playerHitaudioSource.clip = playerHurtSoundEffect;
        playerHitaudioSource.PlayOneShot(playerHurtSoundEffect, 5f);
    
    }

    void IsPlayerHit() 
    {
        if (ishit) 
        { 
            SoundActivate();
        
        }
    
    }
}
