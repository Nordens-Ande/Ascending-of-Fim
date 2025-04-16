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

    void DisableKinematic()
    {
        foreach(var rigidbody in rigidbodies)
        {
            rigidbody.isKinematic = true;
        }
    }

    public void BecomeRagDoll()
    {
        foreach (var rigidbody in rigidbodies)
        {
            rigidbody.isKinematic = false;
        }
        animator.enabled = false;
    }
}
