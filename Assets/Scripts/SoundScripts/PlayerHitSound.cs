using UnityEngine;

public class PlayerHitSound : MonoBehaviour
{
    public AudioSource playerHitaudioSource;
    public AudioClip playerHurtSoundEffect;
    bool ishit;
    

    void Start()
    {
        playerHitaudioSource = GetComponent<AudioSource>();
        //playerHealth = GetComponent<PlayerHealth>();
    }

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
