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
        IsButtonClicked();
    }

    public void SetUIButtonClick(bool b) 
    { 
        isButtonInUse = b;
    }

    public void IsButtonClicked() 
    {
        
        playButtonSound();
        
    }

    void playButtonSound() 
    { 
        currentAudio.clip = buttonSound;
        currentAudio.PlayOneShot(currentAudio.clip, 5);
    }
}
