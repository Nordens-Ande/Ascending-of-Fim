using UnityEngine;

public class EnemyHitSound : MonoBehaviour
{
    public AudioSource enemyHitAudiosource;
    public AudioClip [] enemyHitAudioclip;
    

    void Start()
    {
        enemyHitAudiosource = GetComponent<AudioSource>();
        

    }
    void Update()
    {
        
    }

    public void EnemyHurtSound() 
    {
        enemyHitAudiosource.clip = enemyHitAudioclip[Random.Range(0, enemyHitAudioclip.Length)];
        enemyHitAudiosource.PlayOneShot(enemyHitAudiosource.clip, 1);
    }
   
}
