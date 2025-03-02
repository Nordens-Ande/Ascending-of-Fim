using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static System.TimeZoneInfo;

public class PlayerAnimator : MonoBehaviour
{
    [Header("Must have references")]
    [SerializeField] Animator Animator;
    [SerializeField] Transform RotationNode;
    Rigidbody characterRB;

    [Space]

    [Header("Animation Values")]
    [SerializeField] float TransitionTime = 0.15f;
    string currentState;
    Vector2 inputDirection;



    void Start()
    {
        characterRB = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Animator == null)
            return;

        Vector3 moveVector = characterRB.linearVelocity;
        if (moveVector.magnitude < 1f && inputDirection == Vector2.zero)
        {
            ChangeAnimatonState(TransitionTime, false, "Rifle Idle", "Rifle Idle2");
            Animator.SetFloat("Speed", 1);
            return;
        }
        else
        {
            Animator.SetFloat("Speed", moveVector.magnitude/characterRB.maxLinearVelocity);
        }

        Vector3 moveDirection = moveVector.normalized;
        Vector3 forward = RotationNode.forward;
        Vector3 right = RotationNode.right;

        float forwardDot = Vector3.Dot(forward, moveDirection);
        float rightDot = Vector3.Dot(right, moveDirection);

        string moveState = (forwardDot, rightDot) switch
        {
            (>0.5f, >0.5f) => "Run Forward Right",
            (>0.5f, <-0.5f) => "Run Forward Left",
            (<-0.5f, >0.5f) => "Run Backward Right",
            (<-0.5f, <-0.5f) => "Run Backward Left",
            (>0.7f, _) => "Run Forward",
            (<-0.7f, _) => "Run Backwards",
            (_, >0.7f) => "Run Right",
            (_, <-0.7f) => "Run Left",
            (0, 0) => "Rifle Idle",
            _ => currentState
        };

        ChangeAnimatonState(TransitionTime, true, moveState);
    }



    private void ChangeAnimatonState(float transitionTime, bool getCurrentFrame, string newState)
    {
        //Hindrar att animationen kör om och 'avbryts'
        if (currentState == newState) return;

        //Hämtar nuvarande position i klippet
        float normalizedTime = 0;
        if (getCurrentFrame)
            normalizedTime = Animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1f;

        //Spelar och betsämmer animations state från newState
        Animator.CrossFadeInFixedTime(newState, transitionTime, 0, normalizedTime);
        currentState = newState;
    }
    private void ChangeAnimatonState(float transitionTime, bool getCurrentFrame, params string[] newStates)
    {
        //Hindrar att animationen kör om och 'avbryts'
        if (currentState == newStates[0]) return;

        //Hämtar nuvarande position i klippet
        float normalizedTime = 0;
        if (getCurrentFrame)
            normalizedTime = Animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1f;

        //Spelar och betsämmer animations state från newState
        Animator.CrossFadeInFixedTime(newStates[Random.Range(0, newStates.Length)], transitionTime, 0, normalizedTime);
        currentState = newStates[0];
    }



    private void OnMove(InputValue input)
    {
        inputDirection = input.Get<Vector2>();
    }
}
