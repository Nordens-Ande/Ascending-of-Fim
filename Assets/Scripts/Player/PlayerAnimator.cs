using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using static System.TimeZoneInfo;

public class PlayerAnimator : MonoBehaviour
{
    [Header("Must have references")]
    [SerializeField] Animator Animator;
    [SerializeField] Transform RotationNode;
    Rigidbody characterRB;

    [Space]

    [Header("Animation Values")]
    [SerializeField] float TransitionTime = 0.3f;
    string[] currentState;
    Vector2 inputDirection;



    void Start()
    {
        characterRB = GetComponent<Rigidbody>();
        currentState = new string[2];
    }

    void Update()
    {
        if (Animator == null)
            return;

        Vector3 moveVector = characterRB.linearVelocity;
        if (moveVector.magnitude < 1f && inputDirection == Vector2.zero)
        {
            ChangeAnimatonState(TransitionTime, 0, false, "Rifle Idle", "Rifle Idle2");
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
            _ => "Rifle Idle"
        };

        ChangeAnimatonState(TransitionTime, 0, true, moveState);
    }



    private void ChangeAnimatonState(float transitionTime, int layer, bool getCurrentFrame, string newState)
    {
        //Hindrar att animationen kör om och 'avbryts'
        if (currentState[layer] == newState) return;

        //Hämtar nuvarande position i klippet
        float normalizedTime = 0;
        if (getCurrentFrame)
            normalizedTime = Animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1f;

        //Spelar och betsämmer animations state från newState
        Animator.CrossFadeInFixedTime(newState, transitionTime, layer, normalizedTime);
        currentState[layer] = newState;
    }
    private void ChangeAnimatonState(float transitionTime, int layer, bool getCurrentFrame, params string[] newStates)
    {
        //Hindrar att animationen kör om och 'avbryts'
        if (currentState[layer] == newStates[0]) return;

        //Hämtar nuvarande position i klippet
        float normalizedTime = 0;
        if (getCurrentFrame)
            normalizedTime = Animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1f;

        //Spelar och betsämmer animations state från newState
        Animator.CrossFadeInFixedTime(newStates[Random.Range(0, newStates.Length)], transitionTime, layer, normalizedTime);
        currentState[layer] = newStates[0];
    }



    private void OnMove(InputValue input)
    {
        inputDirection = input.Get<Vector2>();
    }
    private void OnAttack(InputValue input)
    {
        ChangeAnimatonState(TransitionTime, 1, false, "Gunplay1");
    }
    private void OnAttackStop(InputValue input)
    {
        ChangeAnimatonState(TransitionTime, 1, false, "Empty");
    }
}
