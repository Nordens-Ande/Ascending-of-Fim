using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{

    [SerializeField] float movementSpeed;

    Rigidbody characterRB;

    private Vector3 movementInput;
    private Vector3 movementVector;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        characterRB = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        ApplyMovement();
    }

    private void ApplyMovement()
    {
        movementVector.x = movementInput.x * movementSpeed;
        movementVector.z = movementInput.y * movementSpeed;
        movementVector.y = 0;
        movementVector *= Time.deltaTime;

        characterRB.AddForce(movementVector, ForceMode.Impulse);
    }


    public void OnMove(InputValue inputValue)
    {
        movementInput = inputValue.Get<Vector2>();
    }
}
