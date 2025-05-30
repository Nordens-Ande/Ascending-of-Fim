using UnityEngine;

public class SceneRestarScript : MonoBehaviour
{
    private SceneHandler sceneHandler;


    void Start()
    {
        sceneHandler = FindFirstObjectByType<SceneHandler>();
        sceneHandler.LoadMainScene();
    }

}
