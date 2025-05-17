using FirstGearGames.SmoothCameraShaker;
using UnityEngine;
using FirstGearGames.SmoothCameraShaker;

public class PlayerHealth : MonoBehaviour
{
    int health;
    int maxHealth = 100;
    [SerializeField] HUDHandler hudHandler;
    [SerializeField] PlayerDeathController playerDeathController;
    [SerializeField] PlayerHitSound hitsound;

    void Start()
    {
        health = maxHealth;
        PlayerStats.maxHp = maxHealth;
        hudHandler.setMaxHealth(health);
        hudHandler.setHealth(health);
        PlayerStats.hp = health;
    }


    public void ApplyDamage(int damage)
    {
        health -= damage/4;
        hitsound.PlayerHitSoundActivate();
        PlayerStats.hp = health;

        //effects
        if (hudHandler != null)
        {
            hudHandler.setHealth(health);
            hudHandler.hitVisualUI(0.7f);
            hudHandler.FimShotShake();
        }
        

        if(health < 0)
        {
            playerDeathController.PlayerDead();
        }
    }

    public void AddHealth(int addHealth)
    {
        if(health + addHealth > maxHealth)
        {
            //nothing happens cause you can't have more than max hp
        }
        else
        {
            health += addHealth;
            hudHandler.addHealth(addHealth);
            PlayerStats.hp = health;
        }
    }



    public void SetHealth(int newHealth)
    {
        health = newHealth;
        playerDeathController.PlayerRevive();
        PlayerStats.hp = health;

        if (hudHandler != null)
        {
            hudHandler.setHealth((int)newHealth);
        }
    }

    public void resetHealth()
    {
        health = maxHealth;
        playerDeathController.PlayerRevive();
        PlayerStats.hp = health;

        if (hudHandler != null)
        {
            hudHandler.setHealth((int)maxHealth);
        }
    }
}
