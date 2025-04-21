using UnityEngine;

public class SoundManagerScript : MonoBehaviour
{
    AudioSource backgroundMusic;
    AudioSource elevatorMusic;
    bool isPaused;
    bool isPlaying;

    void Start()
    {

        backgroundMusic = GetComponent<AudioSource>();
        elevatorMusic = GetComponent<AudioSource>();
        isPlaying = true;
        isPaused = false;
    }

    void Update()
    {
        //Ett system f�r att stanna bakgrundmusik n�r spelet �r pausat
        //Genom att anv�nda bools
        if (isPlaying && !isPaused) 
        { 
            backgroundMusic.Play();
        }
        if (isPaused && !isPlaying)   
        { 
            backgroundMusic.Stop();
        }

        if (Input.GetKeyDown(KeyCode.Escape) && isPlaying == true) 
        { 
            isPlaying = false;
            isPaused = true;
        }
        if (Input.GetKeyDown(KeyCode.Escape) && isPaused == true) 
        { 
            isPlaying = true;
            isPaused = false;
        }
        
    }

    void IsInTheElevator() 
    {
        backgroundMusic.Stop();
        elevatorMusic.Play();
    }
    void IsBackInTheGame() 
    { 
        backgroundMusic.Play();
        elevatorMusic.Stop();
    }

}
