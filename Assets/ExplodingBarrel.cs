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
            // Damage enemies
            if (nearby.TryGetComponent(out EnemyHealth enemy))
            {
                enemy.ApplyDamage(damage);
                Debug.Log("Explosion damaged enemy: " + nearby.name);
            }

            // Damage player
            if (nearby.CompareTag("Player") && nearby.TryGetComponent(out PlayerHealth player))
            {
                player.ApplyDamage(damage);
                Debug.Log("Explosion damaged player: " + nearby.name);
            }

            // Knockback
            if (nearby.attachedRigidbody != null)
            {
                nearby.attachedRigidbody.AddExplosionForce(explosionForce, transform.position, explosionRadius);
                Debug.Log("Knockback applied to: " + nearby.name);
            }

            // Trigger other barrels
            if (nearby.TryGetComponent(out ExplodingBarrel otherBarrel) && otherBarrel != this)
            {
                otherBarrel.TakeDamage();
            }
        }


        Destroy(gameObject);
    }
}