using UnityEngine;
using UnityEngine.InputSystem;


public class UIHandler : MonoBehaviour
{


    [SerializeField] GameObject MenuList;
    [SerializeField] GameObject UI;
    [SerializeField] GameObject HUD;
    [SerializeField] GameObject WholeMenu;

    [SerializeField] GameObject StartMenu;
    [SerializeField] GameObject OptionsMenu;
    [SerializeField] GameObject PauseMenu;
    [SerializeField] GameObject GameOverMenu;
    [SerializeField] GameObject GraphicsMenu;
    [SerializeField] GameObject InputMenu;
    [SerializeField] GameObject GameRuleMenu;
    [SerializeField] GameObject SoundMenu;
    [SerializeField] GameObject BackStoryMenu;
    [SerializeField] GameObject CreditsMenu;
    
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Time.timeScale = 0f;
    }

    public void ResetUI()
    {
        foreach(Transform child in MenuList.transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    public void ToggleUI()
    {
        //checks if the UI is active or not
        bool isUIActive = UI.activeSelf; 
        //sets the opposite of what the ui was, a toggle
        UI.SetActive(!isUIActive);
    }

    public void ToggleHUD()
    {
        bool isHUDActive = HUD.activeSelf;
        HUD.SetActive(!isHUDActive);
    }

    public void ToggleMenu()
    {
        bool isMenuActive = WholeMenu.activeSelf;
        WholeMenu.SetActive(!isMenuActive);
    }

    public void QuitGame()
    {
        //exists the game
        Application.Quit();
    }

    public void StartGame()
    {
        //normal time
        Time.timeScale = 1.0f;
        ToggleUI();
    }

    public void ActivateStartMenu()
    {
        Time.timeScale = 1.0f;
        StartMenu.SetActive(true);
    }

    public void ActivateOptionsMenu()
    {
        Time.timeScale = 0f;
        OptionsMenu.SetActive(true);
    }

    public void OnPause(InputValue inputValue)
    {
        HUD.SetActive(false);
        UI.SetActive(true);
        ResetUI();
        ActivateOptionsMenu();
        Debug.Log("pasue kallad");
    }


    public void ActivatePauseMenu()
    {
        PauseMenu.SetActive(true);
    }

    public void ActivateGameOverMenu()
    {
        GameOverMenu.SetActive(true);
    }

    public void ActivateGraphicsMenu()
    {
        GraphicsMenu.SetActive(true);
    }

    public void ActivateInputMenu()
    {
        InputMenu.SetActive(true);
    }

    public void ActivateGameRuleMenu()
    {
        GameRuleMenu.SetActive(true);
    }

    public void ActivateSoundMenu()
    {
        SoundMenu.SetActive(true); 
    }

    public void ActivateBackStoryMenu()
    {
        BackStoryMenu.SetActive(true);
    }

    public void ActivateCreditsMenu()
    {
        CreditsMenu.SetActive(true);
    }

    public void StopTime()
    {
        Time.timeScale = 0f;
    }

    public void StartTime()
    {
        Time.timeScale = 1f;
    }

    public bool isMenuActive()
    {
        return UI.activeSelf;
    }
}
