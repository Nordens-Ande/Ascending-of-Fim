using UnityEngine;

public class UiButtonSound : MonoBehaviour
{
    public AudioSource currentAudio;
    public AudioClip buttonSound;
    bool isButtonInUse;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentAudio = GetComponent<AudioSource>();
        
    }

    // Update is called once per frame
    void Update()
    {
        IsButtonClicked();
    }

    public void SetUIButtonClick(bool b) 
    { 
        isButtonInUse = b;
    }

    void IsButtonClicked() 
    {
        if (isButtonInUse && Input.GetMouseButtonDown(0)) 
        {
            playButtonSound();
        }
    }

    void playButtonSound() 
    { 
        currentAudio.clip = buttonSound;
        currentAudio.PlayOneShot(currentAudio.clip, 5);
    }
}
