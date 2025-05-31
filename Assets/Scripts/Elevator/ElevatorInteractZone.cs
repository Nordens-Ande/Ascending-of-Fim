using UnityEngine;
using UnityEngine.InputSystem;

public class ElevatorInteractZone : MonoBehaviour
{
    private bool isPlayerInZone = false;
    GameManager gameManager;
    HUDHandler hudHandler;

    private void Start()
    {
        gameManager = GameObject.FindFirstObjectByType<GameManager>();
        hudHandler = GameObject.FindAnyObjectByType<HUDHandler>(UnityEngine.FindObjectsInactive.Include);
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
            hudHandler.setAnnounchment("You need a keycard to use the elevator!", 2);
        }
    }
}

   
    