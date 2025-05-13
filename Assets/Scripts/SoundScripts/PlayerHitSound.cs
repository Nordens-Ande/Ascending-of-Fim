using UnityEngine;

public class PlayerHitSound : MonoBehaviour
{
    public AudioSource playerHitaudioSource;
    public AudioClip [] playerHurtSoundEffect;

    void Start()
    {
        playerHitaudioSource = GetComponent<AudioSource>();
        
    }

    void Update()
    {
        

    }

    public void PlayerHitSoundActivate() 
    { 
        playerHitaudioSource.clip = playerHurtSoundEffect[Random.Range(0, playerHurtSoundEffect.Length)];
        playerHitaudioSource.PlayOneShot(playerHitaudioSource.clip, 1);
        
    }

    //Denna metod ger ett hurtsound vid varje fj�rdedel av livet som har tagit borts
    //Deadmansswitch anv�nds f�r att hela tiden g�ra s� att ljudet kan anv�ndas igen
    
}
