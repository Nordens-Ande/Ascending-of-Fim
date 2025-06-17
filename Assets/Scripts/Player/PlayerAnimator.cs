using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using static System.TimeZoneInfo;

//Denna klassen ansvarar för vilken animation som ska spelas på spelaren. För att underlätta och ta bort behövet att behöva koppla ihop en massor av animationer så ändrar vi animations state-en via kod i detta scriptet.
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

        //Sätter igång en idle-animation. ChangeAnimatonState() har automatiskt inbyggt att om samma animation spelas så skippas den (return), därför funkar det att göra detta i update loopen.
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

        //Definerar och räknar ut rörelse riktningarna
        Vector3 moveDirection = moveVector.normalized;
        Vector3 forward = RotationNode.forward;
        Vector3 right = RotationNode.right;

        float forwardDot = Vector3.Dot(forward, moveDirection);
        float rightDot = Vector3.Dot(right, moveDirection);

        //Jämför hur spelare går, jämfört med vilket håll han tittar åt och kan logiskt bestämma vilken gå animation som ska spelas då.
        //Dvs. 1,0 är framåt, 0.5,0.5 är fråmåt åt höger.
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


    //Metod som ändrar animationen. Här kan man bestämma vilket animations lager (om man har layerMasks), ifall frame-n från den förra animationen är viktigt och ska användas, övergångstid och den nya animationen (där man dessutom kan ange flera animationer som slumpas mellan)
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

        //ChangeAnimatonState(transitionTime, layer, getCurrentFrame, newStates[Random.Range(0, newStates.Length)]);
    }


    //Spelar animationen när nåt av detta sker, move, attack, attackstop.
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
