using UnityEngine;

public class SoundManagerScript : MonoBehaviour
{
    AudioSource backgroundMusic;
    bool isPaused;
    bool isPlaying;

    void Start()
    {
        backgroundMusic = GetComponent<AudioSource>();
        isPlaying = true;
        isPaused = false;
    }

    void Update()
    {
        //Ett system för att stanna bakgrundmusik när spelet är pausat
        //Genom att använda bools
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
}
