using UnityEngine;

public class RagDollController : MonoBehaviour
{
    Rigidbody[] rigidbodies;
    [SerializeField] Animator animator;

    void Awake()
    {
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        DisableKinematic();
    }

    void DisableKinematic() //disable the kinematics of the rigidbodies for the ragdoll
    {
        foreach(var rigidbody in rigidbodies)
        {
            rigidbody.isKinematic = true;
        }
        if(gameObject.CompareTag("Player")) // player has a different structure than enemies and need to enable the players "main" rigidbody again
        {
            gameObject.GetComponent<Rigidbody>().isKinematic = false;
        }
    }

    public void BecomeRagDoll() // enable all kinematics for the ragdoll, and turn off animator to enable ragdoll
    {
        foreach (var rigidbody in rigidbodies)
        {
            rigidbody.isKinematic = false;
        }
        animator.enabled = false;
    }

    public void NoLongerRagDoll()
    {
        foreach (var rigidbody in rigidbodies)
        {
            rigidbody.isKinematic = true;
        }
        animator.enabled = true;
    }
}
