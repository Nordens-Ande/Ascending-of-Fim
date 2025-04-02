using UnityEngine;

public class SoundEffectsPlayer : MonoBehaviour
{
    public AudioSource CurrentSoundEffect;
    public AudioClip soundEffectShot, soundEffectWalk, soundEffectTalk;

    public void Start()
    {
        CurrentSoundEffect = GetComponent<AudioSource>();
    }
    public void Update()
    {
        playSoundOnWalk();

        isActivelyShooting();

        beginToTalk();
    }
    //Vad som ska spelas vid effekterna
    //Genom att använda playoneshoot kan vi navigera volymer 
    //När flera än en effekt är igång samtidigt
    public void shooting()
    {
        CurrentSoundEffect.clip = soundEffectShot;
        CurrentSoundEffect.PlayOneShot(CurrentSoundEffect.clip, 5);
    }
    public void walking() 
    {
        CurrentSoundEffect.clip = soundEffectWalk;
        //CurrentSoundEffect.Play();
        CurrentSoundEffect.PlayOneShot(CurrentSoundEffect.clip, 3);
    }
    public void talking() 
    {
        CurrentSoundEffect.clip = soundEffectTalk;
        //CurrentSoundEffect.Play();
        CurrentSoundEffect.PlayOneShot(CurrentSoundEffect.clip, 7);
    }

    //Vad som ska uppdateras 
    public void playSoundOnWalk() 
    {
        if (Input.GetKey(KeyCode.W))
        {
            walking();
        }
        
        if (Input.GetKey(KeyCode.S))
        {
            walking();
        }
        if (Input.GetKey(KeyCode.A))
        {
            walking();
        }
        if (Input.GetKey(KeyCode.D))
        {
            walking();
        }
    }
    public void isActivelyShooting() 
    { 
        if (Input.GetMouseButton(0)) 
        { 
            shooting();
        }
        
    }
    public void beginToTalk() 
    {
        if (Random.Range(0, 1000) == 1) 
        { 
            talking();
        }
    }
}
