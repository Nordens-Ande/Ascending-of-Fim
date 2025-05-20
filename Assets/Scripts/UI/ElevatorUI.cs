using TMPro;
using UnityEngine;

public class ElevatorUI : MonoBehaviour
{
    private announcement announcement;

    [SerializeField] int price10;
    [SerializeField] int price5;
    [SerializeField] int price1;
    [Space]
    [SerializeField] int health10;
    [SerializeField] int health5;
    [SerializeField] int health1;
    [Space]
    [SerializeField] private TextMeshProUGUI text10;
    [SerializeField] private TextMeshProUGUI text5;
    [SerializeField] private TextMeshProUGUI text1;
    [Space]
    [SerializeField] private TextMeshProUGUI priceText10;
    [SerializeField] private TextMeshProUGUI priceText5;
    [SerializeField] private TextMeshProUGUI priceText1;
    [Space]
    [SerializeField] HealthBar healthBar;
    [SerializeField] Money moneyScript;

    void Start()
    {
        announcement = FindFirstObjectByType<announcement>();

        text10.text = price10.ToString();
        text5.text = price5.ToString();
        text1.text = price1.ToString();

        priceText10.text = $"{health10.ToString()} Healthpoints";
        priceText5.text = $"{health5.ToString()} Healthpoints";
        priceText1.text = $"{health1.ToString()} Healthpoints";

        healthBar.setMaxHealth(PlayerStats.maxHp);
        healthBar.SetHealth(PlayerStats.hp);
        moneyScript.setNumber(PlayerStats.money);
        PlayerStats.gameHasStarted = true;
    }

    public void add10hp()
    {
        //kollar så spelaren har tillräckligt med pengar och tillräckligt lite hp
        if(PlayerStats.money >= price10 && PlayerStats.hp + health10 < PlayerStats.maxHp)
        {
            PlayerStats.hp = PlayerStats.hp + health10;
            PlayerStats.money -= price10;

            healthBar.SetHealth(PlayerStats.hp);
            moneyScript.setNumber(PlayerStats.money);
        }

        notEnoughFunds(price10);
        tooMuchHealth(health10);
        
    }

    public void add5hp()
    {
        //kollar så spelaren har tillräckligt med pengar och tillräckligt lite hp
        if (PlayerStats.money >= price5 && PlayerStats.hp + health5 <= PlayerStats.maxHp)
        {
            PlayerStats.hp = PlayerStats.hp + health5;
            PlayerStats.money -= price5;

            healthBar.SetHealth(PlayerStats.hp);
            moneyScript.setNumber(PlayerStats.money);
        }

        notEnoughFunds(price5);
        tooMuchHealth(health5);
    }

    public void add1hp()
    {
        //kollar så spelaren har tillräckligt med pengar och tillräckligt lite hp
        if (PlayerStats.money >= price1 && PlayerStats.hp + health1 <= PlayerStats.maxHp)
        {
            PlayerStats.hp = PlayerStats.hp + health1;
            PlayerStats.money -= price1;

            healthBar.SetHealth(PlayerStats.hp);
            moneyScript.setNumber(PlayerStats.money);
        }

        notEnoughFunds(price1);
        tooMuchHealth(health1);
    }


    private void notEnoughFunds(int price)
    {
        if (PlayerStats.money < price)
        {
            announcement.SetAnnouncementText($"Not enough funds", 2f);
        }
    }

    private void tooMuchHealth(int health)
    {
        if (PlayerStats.hp + health > PlayerStats.maxHp)
        {
            announcement.SetAnnouncementText($"you can't have more than {PlayerStats.maxHp} hp", 2f);
        }
    }

}


