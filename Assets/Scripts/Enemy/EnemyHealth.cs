using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    int health;

    [SerializeField] CheckIfEnemyDead enemyDeathScript;
    [SerializeField] EnemyHitSound enemyHitSound;

    void Start()
    {
        health = 50;
    }

    public void ApplyDamage(int damage)
    {
        health -= damage;
        enemyHitSound.EnemyHurtSound();
        if(health <= 0)
        {
            enemyDeathScript.EnemyDead();
        }
    }
}
