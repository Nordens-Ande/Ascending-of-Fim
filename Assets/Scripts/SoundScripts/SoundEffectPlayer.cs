using UnityEngine;

public class SoundEffectsPlayer : MonoBehaviour
{
    public AudioSource CurrentSoundEffect;  
    public AudioClip soundEffectShot;
    public AudioClip[] playerVoicelines;
    int rand;
    bool playershooting;
    PlayerShoot playershoot;

    public void Start()
    {
        CurrentSoundEffect = GetComponent<AudioSource>();
        playershooting = GetComponent<PlayerShoot>().isShooting;
    }
    public void Update()
    {
        rand = Random.Range(0, 10000);

        //isActivelyShooting();

        beginToTalk();
    }
    //Vad som ska spelas vid effekterna
    //Genom att använda playoneshoot kan vi navigera volymer 
    //När flera än en effekt är igång samtidigt
    public void shooting()
    {
        
        CurrentSoundEffect.clip = soundEffectShot;
        CurrentSoundEffect.PlayOneShot(CurrentSoundEffect.clip, 15f);
        

    }
    public void getShooting() 
    {
        shooting();
    
    }

    public void talking() 
    {
        CurrentSoundEffect.clip = playerVoicelines[Random.Range(0, playerVoicelines.Length)];
        CurrentSoundEffect.PlayOneShot(CurrentSoundEffect.clip, 3);
    }

    public void isActivelyShooting() 
    { 
        if (Input.GetMouseButtonDown(0)) 
        { 
            shooting();
        }
        
    }
    public void beginToTalk() 
    {
        if (rand == 1) 
        { 
            talking();
        }
    }
}
