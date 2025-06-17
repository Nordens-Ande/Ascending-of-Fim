
using UnityEngine;

public class SoundEffectsEnemy : MonoBehaviour
{
    public AudioSource CurrentSoundEffect;
    public AudioClip soundEffectShot;
    public AudioClip soundEffectShotgun;

    public bool EnemyIsShooting;

    

    public void Start()
    {
        CurrentSoundEffect = GetComponent<AudioSource>();
    }

    public void SetIsShooting(bool b)
    { 
        EnemyIsShooting = b;
    
    }
    public void Update()
    {
    }

    //Alla skjut ljuden som hämtas finns här 
    public void shooting()
    {
        CurrentSoundEffect.clip = soundEffectShot;
        CurrentSoundEffect.Play();

    }
    public void shotgunShoot() 
    {
        CurrentSoundEffect.clip = soundEffectShotgun;
        CurrentSoundEffect.PlayOneShot(CurrentSoundEffect.clip, 0.2f);

    }

    public void PlayShootingSound() 
    {
        CurrentSoundEffect.clip = soundEffectShot;
        CurrentSoundEffect.PlayOneShot(CurrentSoundEffect.clip, 1f);

    }
    
}
