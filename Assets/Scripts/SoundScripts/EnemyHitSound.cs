using UnityEngine;

public class EnemyHitSound : MonoBehaviour
{
    AudioSource enemyHitAudiosource;
    AudioClip enemyHitAudioclip;
    bool isEnemyHurting;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemyHitAudiosource = GetComponent<AudioSource>();
        //isEnemyHurting = GetComponent<Scriptet där boolen finns när ray träffar fiender>().boolens namn i scriptet;

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
