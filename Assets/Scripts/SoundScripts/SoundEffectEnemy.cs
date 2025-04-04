using UnityEngine;

public class SoundEffectsEnemy : MonoBehaviour
{
    public AudioSource CurrentSoundEffect;
    public AudioClip soundEffectShot, soundEffectWalk, soundEffectTalk;
    int voiceLine;
    public bool EnemyIsMoving;
    public bool EnemyIsShooting;
    

    public void Start()
    {
        CurrentSoundEffect = GetComponent<AudioSource>();
        //EnemyIsMoving = GetComponent<SoundEffectsPlayer>().namnetP�BoolenIEnemyscript;
        //EnemyIsShooting = GetComponent<SoundEffectsPlayer>().namnetP�BoolenIEnemyscript;

    }
    public void Update()
    {
        voiceLine = Random.Range(0, 10000);
        playSoundOnWalk();
        isActivelyShooting();
        beginToTalk();
        //EnemyIsMoving = GetComponent<SoundEffectsPlayer>().namnetP�BoolenIEnemyscript;
        //EnemyIsShooting = GetComponent<SoundEffectsPlayer>().namnetP�BoolenIEnemyscript;
    }

    //Vilka ljud till vilka h�ndelser
    public void shooting()
    {
        CurrentSoundEffect.clip = soundEffectShot;
        CurrentSoundEffect.PlayOneShot(CurrentSoundEffect.clip, 1f);
    }
    public void walking()
    {
        CurrentSoundEffect.clip = soundEffectWalk;
        CurrentSoundEffect.PlayOneShot(CurrentSoundEffect.clip,1f);
    }
    public void talking()
    {
        CurrentSoundEffect.clip = soundEffectTalk;
        CurrentSoundEffect.PlayOneShot(CurrentSoundEffect.clip,1f);
    }

    //Uppdateringar f�r varje metod
    public void playSoundOnWalk()
    {
        if (EnemyIsMoving == true) 
        { 
            walking();
        }
    }
    public void isActivelyShooting()
    {
        if (EnemyIsShooting == true) 
        {
            shooting();
        }
    }
    public void beginToTalk()
    {
        if (voiceLine == 1)
        {
            talking();
        }
    }
}
