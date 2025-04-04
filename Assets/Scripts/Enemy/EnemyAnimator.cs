using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    [Header("Must have references")]
    [SerializeField] Animator Animator;
    [SerializeField] Transform RotationNode;
    Rigidbody characterRB;

    [Space]

    [Header("Animation Values")]
    [SerializeField] float TransitionTime = 0.3f;
    string[] currentState;

    private Vector3 lastPosition;

    void Start()
    {
        characterRB = GetComponent<Rigidbody>();
        currentState = new string[2];
    }
    
    void Update()
    {
        if (Animator == null)
            return;

        //Vector3 moveVector = characterRB.linearVelocity;
        Vector3 moveVector = transform.position - lastPosition;
        moveVector.y = 0;
        lastPosition = transform.position;

        if (moveVector.magnitude == 0f)
        {
            ChangeAnimatonState(TransitionTime, 0, false, "Rifle Idle1");
            //Animator.SetFloat("Speed", 1);
            return;
        }
        else
        {
            Animator.SetFloat("Speed", moveVector.magnitude / (3 * Time.deltaTime));
        }

        Vector3 moveDirection = moveVector.normalized;
        Vector3 forward = RotationNode.forward;
        Vector3 right = RotationNode.right;

        float forwardDot = Vector3.Dot(forward, moveDirection);
        float rightDot = Vector3.Dot(right, moveDirection);

        string moveState = (forwardDot, rightDot) switch
        {
            ( > 0.7f, _) => "Run Forward",
            ( < -0.7f, _) => "Run Backward",
            (_, > 0.7f) => "Run Right",
            (_, < -0.7f) => "Run Left",
            _ => "Rifle Idle1"
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

        Debug.Log(newState);
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
}
