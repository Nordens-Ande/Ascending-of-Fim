using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    int health;

    void Start()
    {
        health = 50;
    }

    public void ApplyDamage(int damage)
    {
        health -= damage;
    }

    void Update()
    {
        if(health < 0)
        {
            Destroy(gameObject);
        }
    }
}
