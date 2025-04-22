using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    int health;

    [SerializeField] CheckIfEnemyDead enemyDeathScript;

    void Start()
    {
        health = 50;
    }

    public void ApplyDamage(int damage)
    {
        health -= damage;
        if(health < 0)
        {
            enemyDeathScript.EnemyDead();
        }
    }
}
