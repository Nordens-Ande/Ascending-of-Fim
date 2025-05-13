using NUnit.Framework.Internal;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
    [SerializeField] private string main;
    [SerializeField] private string elevator;


    //public void LoadSceneByName(string sceneName)
    //{
    //    if (Application.CanStreamedLevelBeLoaded(sceneName))
    //    {
    //        SceneManager.LoadScene(sceneName);
    //    }
    //    else
    //    {
    //        Debug.LogWarning($"Scene '{sceneName}' not found!");
    //    }
    //}

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
        if (Application.CanStreamedLevelBeLoaded(1))
        {
            SceneManager.LoadScene(1);
        }
        else
        {
            Debug.LogWarning("something is wrong and the Elevator scene cannot be loaded");
        }
    }


}
