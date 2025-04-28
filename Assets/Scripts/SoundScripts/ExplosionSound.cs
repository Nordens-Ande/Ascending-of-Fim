using UnityEngine;

public class ExplosionSound : MonoBehaviour
{
    public AudioSource audiosource;
    public AudioClip explosionSound;
    public bool isExploding;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audiosource = GetComponent<AudioSource>();
        isExploding = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        IsExploding();
        
    }

    public void SetToExplode(bool explode) 
    { 
        isExploding = explode;
    
    }

    void IsExploding() 
    { 
        if (isExploding == true) 
        {
            PlayExplosion();
        
        }
    
    }

    void PlayExplosion() 
    { 
        audiosource.clip = explosionSound;
        audiosource.PlayOneShot(explosionSound, 10);
    
    }
}
