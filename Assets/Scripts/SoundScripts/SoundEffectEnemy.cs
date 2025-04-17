
using UnityEngine;

public class SoundEffectsEnemy : MonoBehaviour
{
    public AudioSource CurrentSoundEffect;
    public AudioClip soundEffectShot;
    public AudioClip[] enemyVoicelines;
    int voiceLine;
    public bool EnemyIsShooting = true;
    bool deadmansswitch;
    

    public void Start()
    {
        CurrentSoundEffect = GetComponent<AudioSource>();
        //EnemyIsShooting = GetComponent<SoundEffectsPlayer>().namnetPåBoolenIEnemyscript;
        deadmansswitch = false;
    }
    public void Update()
    {
        
        voiceLine = Random.Range(0, 10000);
        
        isActivelyShooting();
        
        beginToTalk();
        //EnemyIsShooting = GetComponent<SoundEffectsPlayer>().namnetPåBoolenIEnemyscript;
    }
    public void shooting()
    {
        CurrentSoundEffect.clip = soundEffectShot;
        //CurrentSoundEffect.PlayOneShot(CurrentSoundEffect.clip, 1f);
        CurrentSoundEffect.Play();

    }
    
    public void talking()
    {
        CurrentSoundEffect.clip = enemyVoicelines[Random.Range(0, enemyVoicelines.Length)];
        CurrentSoundEffect.PlayOneShot(CurrentSoundEffect.clip, 10f);
       
    }

    //Uppdateringar för shooting
    //Deadmanswitch triggar loopen av ljudet 
    //Sedan om enemy slutar skjuta så återställs deadmanswitc
    //Och metoden kan sättas igång igen  
    
    public void isActivelyShooting()
    {
        if (EnemyIsShooting == true && deadmansswitch == false) 
        {
            shooting();
            deadmansswitch = true;
        }
        if(!EnemyIsShooting) 
        { 
            CurrentSoundEffect.Stop();
            deadmansswitch = false;
        
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
