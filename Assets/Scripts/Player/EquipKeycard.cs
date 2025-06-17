using UnityEngine;
using UnityEngine.InputSystem;

public class EquipKeycard : MonoBehaviour
{
    // This script handles the interaction with the keycard when the player presses the interact button.
    // It uses raycasting to detect the keycard in the player's view and equips it if found.

    [Header("Ray settings")]
    [SerializeField][Range(0.0f, 2.0f)] private float rayLengt;
    [SerializeField] private Vector3 rayOffset; //f�r att flytta Ray upp�t s� att den hamnar r�tt med Fim
    [SerializeField] private LayerMask keycardMask;
    [SerializeField] public Transform orientationObject;
    [SerializeField] SoundEffectsPlayer SEB;
    private RaycastHit topRayHitInfo;

    public GameObject Keycard;
    private KeycardScript keycardScript;
    
    
    public static EquipKeycard Instance { get; private set; } //Singleton pattern
    public bool hasKeycard { get; private set; }

    private void Start()
    {
        hasKeycard = false;
        
    }

    private void Update()
    {
        
    }

    // This method is called when the player presses the interact button.
    // It checks if the player is looking at a keycard and equips it if found.
    public void OnInteract(InputValue inputValue)
    {
        Equip();
        ElevatorInteractZone elevator = GameObject.FindFirstObjectByType<ElevatorInteractZone>();
        if (elevator != null)
        {
            elevator.CheckInteraction();
        }
    }

    // This method handles the raycasting to detect the keycard in the player's view.
    private void RayCastHandler()
    {
        Ray topRay = new Ray(transform.position + rayOffset, orientationObject.forward);

        Debug.DrawRay(transform.position + rayOffset, orientationObject.forward * rayLengt, Color.green);

        Physics.Raycast(topRay, out topRayHitInfo, rayLengt, keycardMask); //F�r att kalla ut Rayen
    }

    // This method equips the keycard if the player is looking at it.
    // It checks if the raycast hit a collider with the "Keycard" tag and calls the Equip method on the KeycardScript component.
    private void Equip()
    {
        RayCastHandler();

        if (topRayHitInfo.collider != null && topRayHitInfo.collider.CompareTag("Keycard"))
        {
            keycardScript = topRayHitInfo.collider.GetComponent<KeycardScript>();
            Keycard = topRayHitInfo.collider.gameObject;
            if (keycardScript != null)
            {
                keycardScript.Equip();
                hasKeycard = true;
                SEB.GotTheKeyCard();

            }
        }
    }
}
