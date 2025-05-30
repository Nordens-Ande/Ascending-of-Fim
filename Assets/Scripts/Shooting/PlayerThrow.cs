using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerThrow : MonoBehaviour
{
    [SerializeField] private GameObject grenadePrefab;
    [SerializeField] private Transform throwPoint;
    [SerializeField] private float throwForce = 3;
    [SerializeField] private int maxGrenades = 3;

    private int grenadesLeft;

    private void Start()
    {
        grenadesLeft = maxGrenades;
    }

    private void OnThrow(InputValue value)
    {
        if (grenadesLeft > 0)
        {
            ThrowGrenade();
            grenadesLeft--;
            Debug.Log("Grenades left: " + grenadesLeft);
        }
        else
        {
            Debug.Log("No grenades left!");
        }
    }

    private void ThrowGrenade()
    {
        if (grenadePrefab == null || throwPoint == null) return;

        GameObject grenade = Instantiate(grenadePrefab, throwPoint.position, Quaternion.identity);
        Rigidbody rb = grenade.GetComponent<Rigidbody>();

        if (rb != null)
        {
            Vector3 throwDirection = throwPoint.forward + Vector3.up * 0.5f;
            rb.AddForce(throwDirection.normalized * throwForce, ForceMode.Impulse);
        }
    }
}
