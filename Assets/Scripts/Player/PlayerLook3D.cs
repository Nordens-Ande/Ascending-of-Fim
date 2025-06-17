using UnityEngine;
using UnityEngine.InputSystem;

//Denna klassen ser till så att spelaren kan sikta korrekt i miljön.
public class IsometricAiming : MonoBehaviour
{
    [Header("Aim")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private bool ignoreHeight = true;

    [Space]

    [Header("Must have references")]
    [SerializeField] private Transform rotObject;
    [SerializeField] private Camera camera;


    private Vector2 mouseVector;


    private void Update()
    {
        Aim();
    }


    //Här siktar vi
    private void Aim()
    {
        var (success, position) = GetMousePosition();
        if (success)
        {
            // Calculate the direction
            var direction = position - transform.position;

            // You might want to delete this line.
            if (ignoreHeight)
                direction.y = 0;

            // Make the transform look in the direction.
            rotObject.forward = direction;
        }
    }

    //Hämtar musens position och ger 'succes' ifall den lyckas kollidera med ett objekt av mask-en
    private (bool success, Vector3 position) GetMousePosition()
    {
        var ray = camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, groundMask))
        {
            // The Raycast hit something, return with the position.
            return (success: true, position: hitInfo.point);
        }
        else
        {
            // The Raycast did not hit anything.
            return (success: false, position: Vector3.zero);
        }
    }

    private void OnLook(InputValue input)
    {
        mouseVector = input.Get<Vector2>();
    }

}