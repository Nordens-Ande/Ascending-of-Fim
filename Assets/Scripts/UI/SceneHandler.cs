using NUnit.Framework.Internal;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
    public void LoadMainScene()
    {
        if (Application.CanStreamedLevelBeLoaded(0))
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            Debug.LogWarning("something is wrong and the Main scene cannot be loaded");
        }
    }
    //dadadwqefd

    public void LoadElevatorScene()
    {
        if (Application.CanStreamedLevelBeLoaded("Elevator"))
        {
            SceneManager.LoadScene("Elevator");
        }
        else
        {
            Debug.LogWarning("something is wrong and the Elevator scene cannot be loaded");
        }
    }

    public void LoadRestartScene()
    {
        if (Application.CanStreamedLevelBeLoaded("RestartScene"))
        {
            SceneManager.LoadScene("RestartScene");
        }
        else
        {
            Debug.LogWarning("something is wrong and the RestartScene scene cannot be loaded");
        }
    }

    public void RestartScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (Application.CanStreamedLevelBeLoaded(currentScene.name))
        {
            SceneManager.LoadScene(currentScene.name);
        }
        else
        {
            Debug.LogWarning("something is wrong and the current scene cannot be reloaded");
        }

    }
}
