using UnityEngine;

public class UiButtonSound : MonoBehaviour
{
    public AudioSource currentAudio;
    public AudioClip buttonSound;
    bool isButtonInUse;

    void Start()
    {
        currentAudio = GetComponent<AudioSource>();
        
    }

    void Update()
    {

    }

    public void playButtonSound() 
    {

       currentAudio.clip = buttonSound;
       currentAudio.PlayOneShot(currentAudio.clip, 3);
    }
}
