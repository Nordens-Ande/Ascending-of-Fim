
using UnityEngine;

public class SoundEffectsEnemy : MonoBehaviour
{
    public AudioSource CurrentSoundEffect;
    public AudioClip soundEffectShot;
    public AudioClip[] enemyVoicelines;
    int voiceLine;

    public bool EnemyIsShooting;
    //public EnemyShot = enemyShot;

    bool deadmansswitch;
    

    public void Start()
    {
        //EnemyIsShooting = GetComponent<EnemyShot>().isShooting;
        CurrentSoundEffect = GetComponent<AudioSource>();

        deadmansswitch = false;
    }
    public void SetIsShooting(bool b)
    { 
        EnemyIsShooting = b;
    
    }
    public void Update()
    {
        
        voiceLine = Random.Range(0, 10000);
        
        //isActivelyShooting();
        
        beginToTalk();
    }
    public void shooting()
    {
        CurrentSoundEffect.clip = soundEffectShot;
        CurrentSoundEffect.Play();

    }

    public void PlayShootingSound() 
    {
        CurrentSoundEffect.clip = soundEffectShot;
        CurrentSoundEffect.PlayOneShot(CurrentSoundEffect.clip, 8f);

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
