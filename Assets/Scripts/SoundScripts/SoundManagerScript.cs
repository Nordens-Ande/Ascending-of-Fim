using Unity.VisualScripting;
using UnityEngine;

public class SoundManagerScript : MonoBehaviour
{
    AudioSource backgroundMusic;
    public AudioClip UIbackgroundMusic;
    public AudioClip mainBackgroundMusic;
    AudioSource elevatorMusic;
    bool isPaused;
    bool isPlaying;
    [SerializeField] HUDHandler hudHandler;

    void Start()
    {

        backgroundMusic = GetComponent<AudioSource>();
        elevatorMusic = GetComponent<AudioSource>();
        //hudHandler = FindAnyObjectByType<HUDHandler>();
        isPlaying = false;
        isPaused = true;
    }

    void Update() 
    {
        
        //Ett system för att stanna bakgrundmusik när spelet är pausat
        //Genom att använda bools och HUDhandlern för att navigera vilken musik

        if (!hudHandler.isMenuActive() && isPlaying == true && !isPaused) //&& hudHandler.isMenuActive() == false) 
        {
            IsBackInTheGame();
        }
        if (hudHandler.isMenuActive() && isPaused == true && !isPlaying) //&& hudHandler.isMenuActive() == true) 
        {
            IsInUI();
        }
        
    }

    //Metod för när spelaren är inne i UI
    //Då stoppas bakgrundsmusiken och UI musiken spelas
    void IsInUI() 
    {
        backgroundMusic.Stop();
        backgroundMusic.clip = UIbackgroundMusic;
        backgroundMusic.Play();
        isPlaying = true;
        isPaused = false;

    }

    //Metod för när spelaren ska tillbaka i spelet från UI
    //Då stoppas UI musiken och bakgrundmusiken börjar spelas igen
    void IsBackInTheGame() 
    {
        backgroundMusic.Stop();
        backgroundMusic.clip = mainBackgroundMusic;
        backgroundMusic.Play();
        isPlaying = false;
        isPaused = true;

    }

}
