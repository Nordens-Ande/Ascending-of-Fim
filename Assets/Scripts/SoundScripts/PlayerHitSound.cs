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

    //Denna metod utgår ifrån att arrayn är fylld med ljud för skada och några voicelines
    //Där alla ljuden ranomiseras och ibland väljs en voiceline
    //Detta ger intrycket att en voiceline randomly spelas när spelaren blir skjuten
    public void PlayerHitSoundActivate() 
    { 
        playerHitaudioSource.clip = playerHurtSoundEffect[Random.Range(0, playerHurtSoundEffect.Length)];
        playerHitaudioSource.PlayOneShot(playerHitaudioSource.clip, 3);
        
    }
    
}
