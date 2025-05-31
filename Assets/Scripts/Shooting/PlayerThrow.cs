using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerThrow : MonoBehaviour
{
    [SerializeField] private GameObject grenadePrefab;
    [SerializeField] private Transform throwPoint;
    [SerializeField] private float throwForce = 3;
    [SerializeField] private int maxGrenades = 3;

    [SerializeField] HUDHandler hudHandler; // Reference to HUDHandler to update grenade count
    [SerializeField] SoundEffectsPlayer SEB;

    private int grenadesLeft;

    private void Start()
    {
        //grenadesLeft = maxGrenades;
        grenadesLeft = PlayerStats.grenades; // Initialize grenades from PlayerStats
    }


    private void Update()
    {
        hudHandler?.setGrenadeCount(grenadesLeft);
    }

    private void OnThrow(InputValue value)
    {
        if (grenadesLeft > 0)
        {
            ThrowGrenade();
            grenadesLeft--;
            Debug.Log("Grenades left: " + grenadesLeft);
            AudioSource.PlayClipAtPoint(SEB.GrenadeExplosion, transform.position);
        }
        else
        {
            hudHandler.setAnnounchment("You have no grenades!", 2);
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
