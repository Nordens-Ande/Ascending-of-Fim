using FirstGearGames.SmoothCameraShaker;
using UnityEngine;
using FirstGearGames.SmoothCameraShaker;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] HUDHandler hudHandler;
    [SerializeField] PlayerDeathController playerDeathController;
    [SerializeField] PlayerHitSound hitsound;

    void Start()
    {
        hudHandler.setMaxHealth(PlayerStats.maxHp);
        hudHandler.setHealth(PlayerStats.hp);
    }


    public void ApplyDamage(int damage)
    {
        PlayerStats.hp -= damage/4;
        hitsound.PlayerHitSoundActivate();

        //effects
        if (hudHandler != null)
        {
            hudHandler.setHealth(PlayerStats.hp);
            hudHandler.hitVisualUI(0.7f);
            hudHandler.FimShotShake();
        }
        

        if(PlayerStats.hp < 0)
        {
            playerDeathController.PlayerDead();
            hudHandler.GameOver();
            PlayerStats.playerHasDied = true;
        }
    }

    public void AddHealth(int addHealth)
    {
        if(PlayerStats.hp + addHealth > PlayerStats.maxHp)
        {
            //nothing happens cause you can't have more than max hp
        }
        else
        {
            PlayerStats.hp += addHealth;
            hudHandler.addHealth(addHealth);
            
        }
    }



    public void SetHealth(int newHealth)
    {
        PlayerStats.hp = newHealth;
        playerDeathController.PlayerRevive();

        if (hudHandler != null)
        {
            hudHandler.setHealth((int)newHealth);
        }
    }

    public void resetHealth()
    {
        PlayerStats.hp = PlayerStats.maxHp;
        playerDeathController.PlayerRevive();

        if (hudHandler != null)
        {
            hudHandler.setHealth((int)PlayerStats.maxHp);
        }
    }
}
