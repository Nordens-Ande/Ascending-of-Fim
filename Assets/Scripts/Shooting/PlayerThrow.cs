using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerThrow : MonoBehaviour
{
    [SerializeField] private GameObject grenadePrefab;
    [SerializeField] private Transform throwPoint;
    [SerializeField] private float throwForce = 3;
    [SerializeField] private float throwCooldown = 2;
    //[SerializeField] private int maxGrenades = 3;

    [SerializeField] HUDHandler hudHandler; // Reference to HUDHandler to update grenade count
    [SerializeField] SoundEffectsPlayer SEB; 

    private float cooldownTimer = 0;
    private int grenadesLeft;

    private void Start()
    {

    }


    private void Update()
    {
        hudHandler?.setGrenadeCount(PlayerStats.grenades);
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }

    // Run this method when the Throw command is called in agme
    private void OnThrow(InputValue value)
    {
        // Check if cooldown has passed
        if (cooldownTimer > 0)
        {
            Debug.Log("Grenade cooldown not over. Time left: " +  cooldownTimer);
            return;
        }

        // If the cooldown has passed and the player has grenades, call ThrowGrenade method, update the amount of grenades left and start the cooldown timer
        if (PlayerStats.grenades > 0)
        {
            ThrowGrenade();
            PlayerStats.grenades--;
            cooldownTimer = throwCooldown;
            Debug.Log("Grenades left: " + PlayerStats.grenades);
            AudioSource.PlayClipAtPoint(SEB.GrenadeExplosion, transform.position);
        }
        else
        {
            hudHandler.setAnnounchment("You have no grenades!", 2);
            Debug.Log("No grenades left!");
        }
    }

    // Instantiate a grenade prefab and apply physics to throw it
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
