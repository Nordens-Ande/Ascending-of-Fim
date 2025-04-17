using UnityEngine;

public class SoundEffectsPlayer : MonoBehaviour
{
    public AudioSource CurrentSoundEffect;  
    public AudioClip soundEffectShot;
    public AudioClip[] playerVoicelines; 
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
    //Genom att anv�nda playoneshoot kan vi navigera volymer 
    //N�r flera �n en effekt �r ig�ng samtidigt
    public void shooting()
    {
        
        CurrentSoundEffect.clip = soundEffectShot;
        CurrentSoundEffect.PlayOneShot(CurrentSoundEffect.clip, 5f);
        

    }

    public void talking() 
    {
        CurrentSoundEffect.clip = playerVoicelines[Random.Range(0, playerVoicelines.Length)];
        CurrentSoundEffect.PlayOneShot(CurrentSoundEffect.clip, 5);
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
