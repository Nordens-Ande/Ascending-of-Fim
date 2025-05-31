using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] float explosionDelay = 1.5f;
    [SerializeField] float explosionRadius = 3.5f;
    [SerializeField] float explosionForce = 750f;
    [SerializeField] float explosionSpeed = 20f;
    [SerializeField] int damage = 30;
    [SerializeField] ParticleSystem explosionEffect;
    [SerializeField] ExplosionSound exSound;

    private HUDHandler hudHandler;
    private bool scoreOnce = false;
    private bool isTriggered = false;

    void Start()
    {
        hudHandler = FindFirstObjectByType<HUDHandler>();
        Invoke(nameof(Explode), explosionDelay);
    }
    public void TakeDamage()
    {
        if (!isTriggered)
        {
            isTriggered = true;
            Invoke(nameof(Explode), explosionDelay);
            AudioSource.PlayClipAtPoint(exSound.explosionSound, transform.position, 5);
        }
    }
    void Explode()
    {
        // explosion visual
        if (explosionEffect)
        {
            GameObject effect = Instantiate(explosionEffect.gameObject, transform.position, Quaternion.identity);
            ParticleSystem effectPS = effect.GetComponent<ParticleSystem>();
            var mainPS = effectPS.main;
            mainPS.startSpeed = explosionSpeed;

            Destroy(effect, 2f);
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        HashSet<GameObject> damagedEnemies = new HashSet<GameObject>();
        HashSet<GameObject> damagedPlayers = new HashSet<GameObject>();

        Debug.Log("Grenade explosion at: " + transform.position + " with radius: " + explosionRadius);
        foreach (Collider nearby in colliders)
        {
            EnemyHealth enemy = nearby.GetComponentInParent<EnemyHealth>();
            if (enemy != null && !damagedEnemies.Contains(enemy.gameObject))
            {
                enemy.ApplyDamage(damage);
                damagedEnemies.Add(enemy.gameObject);
                Debug.Log("Grenade damaged enemy: " + nearby.name);
            }

            PlayerHealth player = nearby.GetComponentInParent<PlayerHealth>();
            if (player != null && !damagedPlayers.Contains(player.gameObject))
            {
                Debug.Log("Found player: " + player.name + "Damage dealt: " + damage);
                player.ApplyDamage(damage);
                damagedPlayers.Add(player.gameObject);
                Debug.Log("Grenade damaged player: " + nearby.name);
            }

            Rigidbody rb = nearby.GetComponentInParent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
                Debug.Log("Knockback applied to: " + rb.gameObject.name);
            }

            // trigger barrels
            if (nearby.TryGetComponent(out ExplodingBarrel barrel))
            {
                barrel.TakeDamage();
            }
        }

        Destroy(gameObject);

        if (hudHandler != null && !scoreOnce)
        {
            scoreOnce = true;
            hudHandler.addScore(100);
        }
    }
}
