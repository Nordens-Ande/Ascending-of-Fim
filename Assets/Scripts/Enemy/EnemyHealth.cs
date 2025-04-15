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
        Debug.Log("damage taken");
        health -= damage;
        Debug.Log(health);
    }

    void Update()
    {
        if(health < 0)
        {
            enemyDeathScript.EnemyDead();
        }
    }
}
