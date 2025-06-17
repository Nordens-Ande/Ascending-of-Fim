using UnityEngine;
using System.Collections;

public class KeycardScript : MonoBehaviour
{
    // This script handles the keycard's rotation and equipping functionality.
    // It rotates the keycard when it is not equipped and disables its collider when equipped.
    [SerializeField] private float keycardRotationSpeed;
    private GameManager gameManager;

    public bool isRotating { get; set; }

    
    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        isRotating = true;
    }

    // This method is called every frame to update the keycard's rotation if it is rotating.
    void Update()
    {
        if (isRotating)
        {
            transform.Rotate(Vector3.up * keycardRotationSpeed * (1 - Mathf.Exp(-keycardRotationSpeed * Time.deltaTime)));
        }
    }

    // This method is called to equip the keycard.
    // It disables the keycard's collider, stops its rotation, and sets it inactive.
    // It also logs a message and notifies the game manager that the keycard has been found.
    public void Equip()
    {
        if (GetComponent<Collider>())
        {
            GetComponent<Collider>().enabled = false;
        }
        isRotating = false;
        gameObject.SetActive(false);

        Debug.LogWarning("Keycard equipped");

        if(gameManager != null)
            gameManager.PlayerFoundKeycard();
    }
}
