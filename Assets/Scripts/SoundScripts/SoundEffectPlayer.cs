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
    public void shooting() //Ljudeffekt för vanligt skott
    {
        
        CurrentSoundEffect.clip = soundEffectShot;
        CurrentSoundEffect.PlayOneShot(CurrentSoundEffect.clip, 1f);
    }

    public void ShotgunShooting() //Ljudeffekt för shotgunskott
    {
        CurrentSoundEffect.clip = soundEffectShotgunShot;
        CurrentSoundEffect.PlayOneShot(CurrentSoundEffect.clip, 0.2f);

    }
    public void ReloadSoundEffect() //Ljudeffekt för när spelare laddar om
    {
        CurrentSoundEffect.clip = ReloadSound;
        CurrentSoundEffect.PlayOneShot(CurrentSoundEffect.clip, 2f);


    }
    public void NeedToRealoadsound() //Ljudeffekt för att indikera att spelaren behöver ladda om
    {
        CurrentSoundEffect.clip = NeedToReloadSound;
        CurrentSoundEffect.PlayOneShot(CurrentSoundEffect.clip, 0.5f);
    }

    public void GrenadeSound() //Ljudeffekt för när spelare har kastat granat
    {
        CurrentSoundEffect.clip = GrenadeExplosion;
        CurrentSoundEffect.PlayOneShot(CurrentSoundEffect.clip, 1f);

    }

    public void GunClick() //Ljudeffekt för när spelaren försöker skjuta utan skott
    {
        CurrentSoundEffect.clip = GunJammed;
        CurrentSoundEffect.PlayOneShot(CurrentSoundEffect.clip, 1f);
    }
    public void GotTheKeyCard() //Ljudeffekt för att indikera att spelaren plockat upp kortet
    {
        CurrentSoundEffect.clip = GotKeyCard;
        CurrentSoundEffect.PlayOneShot(CurrentSoundEffect.clip, 0.5f);

    }
    public void getShooting() 
    {
        shooting();
    
    }

    //Metoder för när spelaren ska säga en voiceline 
    //Metoden använder en random int som hela tiden ändrar värde
    //När värdet är 1 så kommer spelaren säga en voiceline
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
