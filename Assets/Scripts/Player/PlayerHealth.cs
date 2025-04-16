using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    int health;

    [SerializeField] PlayerDeathController playerDeathController;

    void Start()
    {
        health = 100;
    }

    public void ApplyDamage(int damage)
    {
        health -= damage;
        if(health < 0)
        {
            playerDeathController.PlayerDead();
        }
    }
}
