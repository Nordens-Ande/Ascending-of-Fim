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

    void Start()
    {
        characterRB = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Animator == null)
            return;

        Vector3 moveVector = characterRB.linearVelocity;
        if (moveVector.magnitude < 0.1f)
        {
            ChangeAnimatonState("Rifle Idle");
            return;
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

        ChangeAnimatonState(moveState);
    }

    private void ChangeAnimatonState(string newState)
    {
        //Hindrar att animationen kör om och 'avbryts'
        if (currentState == newState) return;
        Debug.Log(newState);

        //Spelar och betsämmer animations state från newState
        Animator.CrossFade(newState, TransitionTime);
        //Animator.Play(newState);
        currentState = newState;
    }
}
