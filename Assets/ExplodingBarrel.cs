using System.Collections.Generic;
using UnityEngine;

public class ExplodingBarrel : MonoBehaviour
{
    [SerializeField] float delay = 2f;
    [SerializeField] float explosionRadius = 7f;
    [SerializeField] float explosionForce = 750f;
    [SerializeField] float explosionSpeed = 20f;
    [SerializeField] int damage = 25;
    [SerializeField] ParticleSystem explosionEffect;

    private HUDHandler hudHandler;
    private bool scoreOnce = false;

    private bool isTriggered = false;

    private void Awake()
    {
        hudHandler = FindFirstObjectByType<HUDHandler>();
    }

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

        // using hashset to track if player/enemy has already been damaged
        HashSet<GameObject> damagedEnemies = new HashSet<GameObject>();
        HashSet<GameObject> damagedPlayers = new HashSet<GameObject>();

        foreach (Collider nearby in colliders)
        {
            // Damage enemies
            EnemyHealth enemy = nearby.GetComponentInParent<EnemyHealth>();
            if (enemy != null && !damagedEnemies.Contains(enemy.gameObject))
            {
                enemy.ApplyDamage(damage);
                damagedEnemies.Add(enemy.gameObject);
                Debug.Log("Explosion damaged enemy: " + nearby.name);
            }

            // Damage player
            PlayerHealth player = nearby.GetComponentInParent<PlayerHealth>();
            if (player != null && !damagedPlayers.Contains(player.gameObject))
            {
                player.ApplyDamage(damage);
                damagedPlayers.Add(player.gameObject);
                Debug.Log("Explosion damaged player: " + nearby.name);
            }

            // Knockback
            Rigidbody rb = nearby.GetComponentInParent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
                Debug.Log("Knockback applied to: " + rb.gameObject.name);
            }

            // Trigger other barrels
            if (nearby.TryGetComponent(out ExplodingBarrel otherBarrel) && otherBarrel != this)
            {
                otherBarrel.TakeDamage();
            } 
        }

        Destroy(gameObject);

        //give score to hud
        if (hudHandler != null && !scoreOnce)
        {
            scoreOnce = true;
            hudHandler.addScore(500);
        }
    }
}