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
    //Metod som hämtas till UI när knapptryck ska ha en ljudeffekt
    public void playButtonSound() 
    {

       currentAudio.clip = buttonSound;
       currentAudio.PlayOneShot(currentAudio.clip, 3);
    }
}
