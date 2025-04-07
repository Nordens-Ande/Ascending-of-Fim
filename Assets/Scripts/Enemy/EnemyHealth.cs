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
        Debug.Log("enemy hit");
    }

    void Update()
    {
        if(health < 0)
        {
            Destroy(gameObject);
        }
    }
}
