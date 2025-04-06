using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    int health;

    void Start()
    {
        health = 100;
    }

    public void ApplyDamage(int damage)
    {
        health -= damage;
    }
    
}
