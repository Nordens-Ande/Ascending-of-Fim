using UnityEngine;

public class ExplodingBarrel : MonoBehaviour
{
    public float delay = 2f;
    public float explosionRadius = 7f;
    public float explosionForce = 750f;
    public int damage = 15;
    public GameObject explosionEffect;

    private bool isTriggered = false;

    public void TakeDamage()
    {
        if (!isTriggered)
        {
            isTriggered = true;
            Invoke(nameof(Explode), delay);
        }
    }

    void Explode()
    {
        Debug.Log("Explode called");
        // explosion particle
        if (explosionEffect)
        {
            GameObject effect = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Destroy(effect, 2f);
        }

        // do damage and knockback to nearby objects
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider nearby in colliders)
        {
            // damage enemies
            EnemyHealth enemy = nearby.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.ApplyDamage(damage);
                Debug.Log("Explosion hit: " + nearby.name);
            }

            // damage player
            if (nearby.CompareTag("Player"))
            {
                PlayerHealth player = nearby.GetComponent<PlayerHealth>();
                if (player != null)
                {
                    player.ApplyDamage(damage);
                    Debug.Log("Explosion hit: " + nearby.name);
                }
            }

            // knockback anything with rigidbody
            Rigidbody rb = nearby.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }

            // trigger other barrels
            ExplodingBarrel otherBarrel = nearby.GetComponent<ExplodingBarrel>();
            if (otherBarrel != null && otherBarrel != this) // avoid triggering itself
            {
                otherBarrel.TakeDamage();
            }
        }
        Destroy(gameObject);
    }
}