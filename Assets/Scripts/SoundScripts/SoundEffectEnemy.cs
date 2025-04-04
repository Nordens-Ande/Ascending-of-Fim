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
        //EnemyIsMoving = GetComponent<SoundEffectsPlayer>().namnetPåBoolenIEnemyscript;
        //EnemyIsShooting = GetComponent<SoundEffectsPlayer>().namnetPåBoolenIEnemyscript;

    }
    public void Update()
    {
        voiceLine = Random.Range(0, 10000);
        playSoundOnWalk();
        isActivelyShooting();
        beginToTalk();
        //EnemyIsMoving = GetComponent<SoundEffectsPlayer>().namnetPåBoolenIEnemyscript;
        //EnemyIsShooting = GetComponent<SoundEffectsPlayer>().namnetPåBoolenIEnemyscript;
    }

    //Vilka ljud till vilka händelser
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

    //Uppdateringar för varje metod
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
