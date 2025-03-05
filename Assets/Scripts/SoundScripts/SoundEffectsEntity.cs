using UnityEngine;

public class SoundEffectsPlayer : MonoBehaviour
{
    public AudioSource CurrentSoundEffect;
    public AudioClip soundEffectShot, soundEffectWalk, soundEffectTalk;
    public void shooting()
    {
        CurrentSoundEffect.clip = soundEffectShot;
        CurrentSoundEffect.Play();
    }
    public void walking() 
    {
        CurrentSoundEffect.clip = soundEffectWalk;
        CurrentSoundEffect.Play();
    }
    public void talking() 
    {
        CurrentSoundEffect.clip = soundEffectTalk;
        CurrentSoundEffect.Play();
    }
    

}
