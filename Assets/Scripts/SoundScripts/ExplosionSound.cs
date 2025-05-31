using UnityEngine;

public class ExplosionSound : MonoBehaviour
{
    public AudioSource audiosource;
    public AudioClip explosionSound;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audiosource = GetComponent<AudioSource>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //Explosion när en barrel blir skjuten
    public void PlayExplosion() 
    { 
        audiosource.clip = explosionSound;
        audiosource.PlayOneShot(explosionSound, 10);
    
    }
}
