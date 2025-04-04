using UnityEngine;
using UnityEngine.InputSystem;


public class UIHandler : MonoBehaviour
{


    [SerializeField] GameObject menuList;
    [SerializeField] GameObject UI;

    [SerializeField] GameObject StartMenu;
    [SerializeField] GameObject OptionsMenu;
    [SerializeField] GameObject PauseMenu;
    [SerializeField] GameObject GameOverMenu;
    [SerializeField] GameObject GraphicsMenu;
    [SerializeField] GameObject InputMenu;
    [SerializeField] GameObject GameRuleMenu;
    [SerializeField] GameObject SoundMenu;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Time.timeScale = 1f;
    }

    public void ResetUI()
    {
        foreach(Transform child in menuList.transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    public void OnPause(InputValue input)
    {
        ToggleUI();
        ResetUI();
        ActivateOptionsMenu();
    }

    public void ToggleUI()
    {
        //checks if the UI is active or not
        bool isUIActive = UI.activeSelf; 
        //sets the opposite of what the ui was, a toggle
        UI.SetActive(!isUIActive);
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
}
