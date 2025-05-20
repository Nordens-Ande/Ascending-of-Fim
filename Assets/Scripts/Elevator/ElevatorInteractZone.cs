using UnityEngine;
using UnityEngine.InputSystem;

public class ElevatorInteractZone : MonoBehaviour
{
    private bool isPlayerInZone = false;

    public void InteractWrapper()
    {
        OnInteract(new InputValue()); // Du kan ignorera värdet eller kopiera logiken till denna metod
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = true;
            Debug.Log("Player entered the elevator interact zone.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = false;
            Debug.Log("Player exited the elevator interact zone.");
        }
    }

    public void OnInteract(InputValue inputValue)
    {
        if(!isPlayerInZone)
        {
            return;
        }

        if(EquipKeycard.Instance.hasKeycard)
        {
            Debug.Log("Interacting with the elevator.");
            // Add your elevator interaction logic here
        }
        else
        {
            Debug.Log("You need a keycard to interact with the elevator.");
        }

    }
}

   
    