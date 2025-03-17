using UnityEngine;

public class SoundEffectsEnemy : MonoBehaviour
{
    public AudioSource CurrentSoundEffect;
    public AudioClip soundEffectShot, soundEffectWalk, soundEffectTalk;
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
        CurrentSoundEffect.PlayOneShot(CurrentSoundEffect.clip,4);
    }
    public void walking()
    {
        CurrentSoundEffect.clip = soundEffectWalk;
        //CurrentSoundEffect.Play();
        CurrentSoundEffect.PlayOneShot(CurrentSoundEffect.clip,2);
    }
    public void talking()
    {
        CurrentSoundEffect.clip = soundEffectTalk;
        //CurrentSoundEffect.Play();
        CurrentSoundEffect.PlayOneShot(CurrentSoundEffect.clip,6);
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
        if (Random.Range(0, 1000) == 1)
        {
            talking();
        }
    }
}
