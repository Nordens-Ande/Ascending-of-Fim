using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{

    [SerializeField] float movementSpeed;


    [Header("Movement")]
    [SerializeField] private float maxSpeed = 8f;
    [SerializeField] private float acceleration = 200f;
    [SerializeField] private AnimationCurve accelerationFactorFromDot;
    [SerializeField] private float maxAccelForce = 150f;
    [SerializeField] private AnimationCurve maxAccelerationForceFactorFromDot;
    [SerializeField] private Vector3 forceScale = new Vector3(1, 0, 1);
    [SerializeField] private float gravityScaleDrop = 10f;


    Rigidbody characterRB;

    private Vector3 movementInput;
    private Vector3 movementVector;

    private Vector3 mUnitGoal;
    private Vector3 mGoalVel;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        characterRB = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(mUnitGoal);
        //ApplyMovement();
        CalculateMovement();
    }

    private void ApplyMovement()
    {
        movementVector.x = movementInput.x * movementSpeed;
        movementVector.z = movementInput.y * movementSpeed;
        movementVector.y = 0;
        movementVector *= Time.deltaTime;

        characterRB.AddForce(movementVector, ForceMode.Impulse);
    }

    private void CalculateMovement()
    {
        Vector3 unitVel = mGoalVel.normalized;

        float velDot = Vector3.Dot(mUnitGoal, unitVel);

        float accel = acceleration * accelerationFactorFromDot.Evaluate(velDot);

        Vector3 goalVel = mUnitGoal * maxSpeed * 1; //(speedfactor)

        mGoalVel = Vector3.MoveTowards(mGoalVel, goalVel, accel * Time.fixedDeltaTime);


        Vector3 neededAccel = (mGoalVel - characterRB.linearVelocity) / Time.fixedDeltaTime;

        float maxAccel = maxAccelForce * maxAccelerationForceFactorFromDot.Evaluate(velDot) * 1; //(factor)

        neededAccel = Vector3.ClampMagnitude(neededAccel, maxAccel);

        characterRB.AddForce(Vector3.Scale(neededAccel * characterRB.mass, forceScale));
    }

    public void OnMove(InputValue inputValue)
    {
        Vector2 inputVector = inputValue.Get<Vector2>();

        if (inputVector.magnitude > 1.0f)
            inputVector.Normalize();

        mUnitGoal = new Vector3(inputVector.x, 0, inputVector.y);
    }
}
