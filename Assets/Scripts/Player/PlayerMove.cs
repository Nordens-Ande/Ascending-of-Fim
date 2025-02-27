using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    Rigidbody characterRB;

    [Header("Movement")]
    public float MoveSpeed;


    Vector3 moveDirection;


    void Start()
    {
        characterRB = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        SpeedLimiter();
    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void Movement()
    {
        characterRB.AddForce(moveDirection * MoveSpeed * 10f, ForceMode.Force);
    }

    private void SpeedLimiter()
    {
        Vector3 currentVel = new Vector3(characterRB.linearVelocity.x, 0, characterRB.linearVelocity.z);

        if (currentVel.magnitude > MoveSpeed)
        {
            Vector3 limitedVel = currentVel.normalized * MoveSpeed;
            characterRB.linearVelocity = new Vector3(limitedVel.x, characterRB.linearVelocity.y, limitedVel.z);
        }
    }

    public void OnMove(InputValue inputValue)
    {
        Vector2 inputVector = inputValue.Get<Vector2>();

        if (inputVector.magnitude > 1.0f)
            inputVector.Normalize();

        moveDirection = new Vector3(inputVector.x, 0, inputVector.y);
    }
}
