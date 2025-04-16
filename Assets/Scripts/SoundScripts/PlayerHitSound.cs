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
        //ishit = GetComponent<Script,där,ray,finns,när,Fim,blir,träffad>().Namnet,på,bool,som,används,när,ray,träffar;
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
