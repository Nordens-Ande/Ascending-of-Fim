using FirstGearGames.SmoothCameraShaker;
using UnityEngine;
using FirstGearGames.SmoothCameraShaker;

public class PlayerHealth : MonoBehaviour
{
    int health;
    int maxHealth = 100;
    [SerializeField] HUDHandler hudHandler;
    [SerializeField] PlayerDeathController playerDeathController;

    void Start()
    {
        health = maxHealth;
        hudHandler.setMaxHealth(health);
        hudHandler.setHealth(health);
    }


    public void ApplyDamage(int damage)
    {
        health -= damage/4;
        
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
        }
      

    }
}
