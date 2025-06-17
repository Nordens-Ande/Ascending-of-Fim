using UnityEngine;
using UnityEngine.InputSystem;

public class ElevatorInteractZone : MonoBehaviour
{ 
    // This script handles the interaction with the elevator when the player enters the designated zone.
    // This script is attached to the elevator interact zone.

    private bool isPlayerInZone = false;
    GameManager gameManager;
    HUDHandler hudHandler;

    private void Start() 
    {
        gameManager = GameObject.FindFirstObjectByType<GameManager>();
        hudHandler = GameObject.FindAnyObjectByType<HUDHandler>(UnityEngine.FindObjectsInactive.Include);
    }

    // This method is called when the player enters the trigger collider of the elevator interact zone.
    // It sets a flag to true, indicating the player is in the zone and can interact with the elevator.
    private void OnTriggerEnter(Collider other) 
    {
        if (other.CompareTag("PlayerHitbox")) 
        {
            isPlayerInZone = true; 
            Debug.Log("Player entered the elevator interact zone.");
        }
    }

    // This method is called when the player exits the trigger collider of the elevator interact zone.
    // It sets the flag to false, indicating the player is no longer in the zone.
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlayerHitbox")) 
        {
            isPlayerInZone = false; 
            Debug.Log("Player exited the elevator interact zone.");
        }
    }

    // This method is called to check if the player can interact with the elevator.
    // It can be called from an input action or UI button to trigger the elevator interaction.
    // It checks if the player is in the zone and if they have a keycard.
    // If they do, it loads the elevator scene; otherwise, it shows a message.
    // If the gameManager is null, it loads the main scene.
    public void CheckInteraction() 
    {
        if (!isPlayerInZone) 
        {
            return;
        }

        if (gameManager != null && gameManager.HasKeycard) 
        {
            SceneHandler sceneHandler = GameObject.FindAnyObjectByType<SceneHandler>(); 
            if (sceneHandler != null)
            {
                sceneHandler.LoadElevatorScene(); 
            }
        }
        else
        {
            hudHandler.setAnnounchment("You need a keycard to use the elevator!", 2);
        }
        
        if(gameManager == null)
        {
            SceneHandler sceneHandler = GameObject.FindAnyObjectByType<SceneHandler>();
            if (sceneHandler != null)
            {
                PlayerStats.resetValues();
                sceneHandler.LoadMainScene();
            }
        }
    }
}

   
    