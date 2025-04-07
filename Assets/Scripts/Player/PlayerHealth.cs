using UnityEngine;

public class PlayerHealth : MonoBehaviour
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
