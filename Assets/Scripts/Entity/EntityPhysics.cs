using Unity.VisualScripting;
using UnityEngine;

public class PlayerPhysics : MonoBehaviour
{
    [Header("Drag values")]
    [SerializeField] float DragHeight;
    [SerializeField] float GroundDrag = 5;
    [SerializeField] float AirDrag = 0;

    [Space]

    [Header("Floating Parameters (buggig)")]
    [SerializeField] bool FloatEnabled = false;
    [SerializeField] float FloatHeight;
    [SerializeField] float FloatSpringHeight;
    [SerializeField] float FloatSpringDamper;

    Rigidbody characterRB;



    void Start()
    {
        characterRB = GetComponent<Rigidbody>();
    }

    void Update()
    {
        DragHandler();
    }

    private void FixedUpdate()
    {
        if (FloatEnabled)
            FloatHandler();
    }



    private void FloatHandler()
    {
        RaycastHit ray;

        if (Physics.Raycast(transform.position, Vector3.down, out ray, FloatHeight))
        {
            Vector3 velocity = characterRB.linearVelocity;
            Vector3 rayDir = transform.TransformDirection(Vector3.down);

            Vector3 otherVel = Vector3.zero;
            Rigidbody hitBody = ray.rigidbody;
            if (hitBody != null)
            {
                otherVel = hitBody.linearVelocity;
            }

            float rayDirVel = Vector3.Dot(rayDir, velocity);
            float otherDirVel = Vector3.Dot(rayDir, otherVel);
            float relVel = rayDirVel - otherDirVel;

            float x = ray.distance - FloatHeight;

            float springForce = (x * FloatSpringHeight) - (relVel * FloatSpringDamper);
            characterRB.AddForce(rayDir * springForce);

            Debug.DrawLine(transform.position, transform.position + (rayDir * springForce), Color.blue);

            if (hitBody != null)
            {
                hitBody.AddForceAtPosition(rayDir * -springForce, ray.point);
            }
        }
    }

    private void DragHandler()
    {
        RaycastHit ray;

        if (Physics.Raycast(transform.position, Vector3.down, out ray, DragHeight))
            characterRB.linearDamping = GroundDrag;
        else 
            characterRB.linearDamping = AirDrag;
    }
}
