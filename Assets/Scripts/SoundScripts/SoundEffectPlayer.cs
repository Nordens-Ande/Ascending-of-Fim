using UnityEngine;

public class SoundEffectsPlayer : MonoBehaviour
{
    public AudioSource CurrentSoundEffect;  
    public AudioClip soundEffectShot;
    public AudioClip soundEffectShotgunShot;
    public AudioClip ReloadSound;
    public AudioClip NeedToReloadSound;
    public AudioClip GrenadeExplosion;
    public AudioClip GunJammed;
    public AudioClip GotKeyCard;
    public AudioClip[] playerVoicelines;
    int rand;

    public void Start()
    {
        CurrentSoundEffect = GetComponent<AudioSource>();
        
    }
    public void Update()
    {
        rand = Random.Range(0, 15000);

        beginToTalk();
    }
    //Vad som ska spelas vid effekterna
    //Genom att använda playoneshoot kan vi navigera volymer 
    //När flera än en effekt är igång samtidigt
    public void shooting()
    {
        
        CurrentSoundEffect.clip = soundEffectShot;
        CurrentSoundEffect.PlayOneShot(CurrentSoundEffect.clip, 1f);
    }

    public void ShotgunShooting() 
    {
        CurrentSoundEffect.clip = soundEffectShotgunShot;
        CurrentSoundEffect.PlayOneShot(CurrentSoundEffect.clip, 0.2f);

    }
    public void ReloadSoundEffect() 
    {
        CurrentSoundEffect.clip = ReloadSound;
        CurrentSoundEffect.PlayOneShot(CurrentSoundEffect.clip, 2f);


    }
    public void NeedToRealoadsound() 
    {
        CurrentSoundEffect.clip = NeedToReloadSound;
        CurrentSoundEffect.PlayOneShot(CurrentSoundEffect.clip, 0.5f);
    }

    public void GrenadeSound() 
    {
        CurrentSoundEffect.clip = GrenadeExplosion;
        CurrentSoundEffect.PlayOneShot(CurrentSoundEffect.clip, 1f);

    }

    public void GunClick() 
    {
        CurrentSoundEffect.clip = GunJammed;
        CurrentSoundEffect.PlayOneShot(CurrentSoundEffect.clip, 1f);
    }
    public void GotTheKeyCard() 
    {
        CurrentSoundEffect.clip = GotKeyCard;
        CurrentSoundEffect.PlayOneShot(CurrentSoundEffect.clip, 0.5f);

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
    public void beginToTalk() 
    {
        if (rand == 1) 
        { 
            talking();
        }
    }
}
