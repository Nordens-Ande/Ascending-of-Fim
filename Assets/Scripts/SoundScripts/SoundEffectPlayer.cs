using UnityEngine;

public class SoundEffectsPlayer : MonoBehaviour
{
    public AudioSource CurrentSoundEffect;  
    public AudioClip soundEffectShot, soundEffectTalk;
    int rand;

    public void Start()
    {
        CurrentSoundEffect = GetComponent<AudioSource>();
    }
    public void Update()
    {
        rand = Random.Range(0, 10000);

        isActivelyShooting();

        beginToTalk();
    }
    //Vad som ska spelas vid effekterna
    //Genom att använda playoneshoot kan vi navigera volymer 
    //När flera än en effekt är igång samtidigt
    public void shooting()
    {
        
        CurrentSoundEffect.clip = soundEffectShot;
        CurrentSoundEffect.PlayOneShot(CurrentSoundEffect.clip, 5f);
        

    }

    public void talking() 
    {
        CurrentSoundEffect.clip = soundEffectTalk;
        CurrentSoundEffect.PlayOneShot(CurrentSoundEffect.clip, 1);
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
