using Unity.VisualScripting;
using UnityEngine;

public class SoundManagerScript : MonoBehaviour
{
    AudioSource backgroundMusic;
    AudioSource elevatorMusic;
    bool isPaused;
    bool isPlaying;
    [SerializeField] HUDHandler hudHandler;

    void Start()
    {

        backgroundMusic = GetComponent<AudioSource>();
        elevatorMusic = GetComponent<AudioSource>();
        hudHandler = FindAnyObjectByType<HUDHandler>();
        isPlaying = true;
        isPaused = false;
    }

    void Update() 
    {
        
        //Ett system för att stanna bakgrundmusik när spelet är pausat
        //Genom att använda bools
        

        if (Input.GetKeyDown(KeyCode.Escape) && isPlaying == true && !isPaused && hudHandler.isMenuActive() == false) 
        {
            backgroundMusic.Stop();
            isPlaying = false;
            isPaused = true;
        }
        if (Input.GetKeyDown(KeyCode.Escape) && isPaused == true && !isPlaying && hudHandler.isMenuActive() == true) 
        {
            backgroundMusic.Play();
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
