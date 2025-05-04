using UnityEngine;

public class ExplodingBarrel : MonoBehaviour
{
    [SerializeField] float delay = 2f;
    [SerializeField] float explosionRadius = 7f;
    [SerializeField] float explosionForce = 750f;
    [SerializeField] float explosionSpeed = 20f;
    [SerializeField] int damage = 15;
    [SerializeField] ParticleSystem explosionEffect;

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
            GameObject effect = Instantiate(explosionEffect.gameObject, transform.position, Quaternion.identity);
            ParticleSystem effectPS = effect.GetComponent<ParticleSystem>();
            var mainPS = effectPS.main;
            mainPS.startSpeed = explosionSpeed;

            Destroy(effect, 2f);
        }

        // do damage and knockback to nearby objects
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider nearby in colliders)
        {
            // Damage enemies
            EnemyHealth enemy = nearby.GetComponentInParent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.ApplyDamage(damage);
                Debug.Log("Explosion damaged enemy: " + nearby.name);
            }

            // Damage player
            PlayerHealth player = nearby.GetComponentInParent<PlayerHealth>();
            if (player != null)
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