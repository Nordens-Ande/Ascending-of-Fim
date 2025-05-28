using UnityEngine;
using UnityEngine.InputSystem;

public class ElevatorInteractZone : MonoBehaviour
{
    private bool isPlayerInZone = false;
    GameManager gameManager;

    private void Start()
    {
        gameManager = GameObject.FindFirstObjectByType<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerHitbox"))
        {
            isPlayerInZone = true;
            Debug.Log("Player entered the elevator interact zone.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlayerHitbox"))
        {
            isPlayerInZone = false;
            Debug.Log("Player exited the elevator interact zone.");
        }
    }

    public void CheckInteraction()
    {
        if (!isPlayerInZone)
        {
            
            return;
        }

        if (gameManager.HasKeycard)
        {
            SceneHandler sceneHandler = GameObject.FindAnyObjectByType<SceneHandler>();
            if(sceneHandler != null)
            {
                sceneHandler.LoadElevatorScene();
            }
        }
        else
        {
            
        }
    }
}

   
    