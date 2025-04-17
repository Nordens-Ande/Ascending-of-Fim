
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
        //EnemyIsShooting = GetComponent<SoundEffectsPlayer>().namnetP�BoolenIEnemyscript;
        deadmansswitch = false;
    }
    public void Update()
    {
        
        voiceLine = Random.Range(0, 10000);
        
        isActivelyShooting();
        
        beginToTalk();
        //EnemyIsShooting = GetComponent<SoundEffectsPlayer>().namnetP�BoolenIEnemyscript;
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

    //Uppdateringar f�r shooting
    //Deadmanswitch triggar loopen av ljudet 
    //Sedan om enemy slutar skjuta s� �terst�lls deadmanswitc
    //Och metoden kan s�ttas ig�ng igen  
    
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
