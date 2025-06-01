using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneHandler : MonoBehaviour
{
    private bool isLoading = false;


    public void LoadMainScene()
    {
        //if (Application.CanStreamedLevelBeLoaded(0))
        //{
        //    SceneManager.LoadScene(0);
        //}
        //else
        //{
        //    Debug.LogWarning("something is wrong and the Main scene cannot be loaded");
        //}
        LoadSceneAsyncByIndex(0);
    }

    public void LoadElevatorScene()
    {
        //if (Application.CanStreamedLevelBeLoaded(1))
        //{
        //    SceneManager.LoadScene(1);
        //}
        //else
        //{
        //    Debug.LogWarning("something is wrong and the Elevator scene cannot be loaded");
        //}
        LoadSceneAsyncByIndex(1);
    }

    public void LoadRestartScene()
    {
        //if (Application.CanStreamedLevelBeLoaded(2))
        //{
        //    SceneManager.LoadScene(2);
        //}
        //else
        //{
        //    Debug.LogWarning("something is wrong and the RestartScene scene cannot be loaded");
        //}
        LoadSceneAsyncByIndex(2);
    }

    public void LoadTutorialScene()
    {
        //if (Application.CanStreamedLevelBeLoaded(3))
        //{
        //    SceneManager.LoadScene(3);
        //}
        //else
        //{
        //    Debug.LogWarning("something is wrong and the tutorial scene cannot be loaded");
        //}
        LoadSceneAsyncByIndex(3);
    }

    public void RestartScene()
    {
        //Scene currentScene = SceneManager.GetActiveScene();
        //if (Application.CanStreamedLevelBeLoaded(currentScene.name))
        //{
        //    SceneManager.LoadScene(currentScene.name);
        //}
        //else
        //{
        //    Debug.LogWarning("something is wrong and the current scene cannot be reloaded");
        //}

        Scene currentScene = SceneManager.GetActiveScene();
        LoadSceneAsyncByIndex(currentScene.buildIndex);
    }


    private void LoadSceneAsyncByIndex(int index)
    {
        if (isLoading) return;

        if (Application.CanStreamedLevelBeLoaded(index))
        {
            StartCoroutine(LoadSceneCoroutine(index));
        }
        else
        {
            Debug.LogWarning($"Scene with index {index} cannot be loaded.");
        }
    }

    private IEnumerator LoadSceneCoroutine(int index)
    {
        isLoading = true;


        yield return new WaitForSeconds(0.25f);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(index);

        // Optional: prevent scene activation until ready
        // asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            // Optional: progress reporting
            // Debug.Log($"Loading progress: {asyncLoad.progress}");

            yield return null;
        }

        isLoading = false;
    }

}
