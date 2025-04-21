using UnityEngine;

public class EnemyHitSound : MonoBehaviour
{
    AudioSource enemyHitAudiosource;
    AudioClip enemyHitAudioclip;
    public bool isEnemyHurting;

    void Start()
    {
        enemyHitAudiosource = GetComponent<AudioSource>();
        //isEnemyHurting = GetComponent<Scriptet d�r boolen finns n�r ray tr�ffar fiender>().boolens namn i scriptet;

    }
    // Update is called once per frame
    void Update()
    {
        IsEnemyHit();
    }
    void EnemyHurtSound() 
    { 
        enemyHitAudiosource.clip = enemyHitAudioclip;
        enemyHitAudiosource.PlayOneShot(enemyHitAudioclip, 5);
        
    
    }

    void IsEnemyHit() 
    {
        if (isEnemyHurting) 
        { 
            EnemyHurtSound();
        
        }
    
    }
}
