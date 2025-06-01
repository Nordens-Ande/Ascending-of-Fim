using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ElevatorUI : MonoBehaviour
{
    private announcement announcement;

    [SerializeField] int price10hp;
    [SerializeField] int price5hp;
    [SerializeField] int price1hp;
    [SerializeField] int price1grenade;
    [Space]
    [SerializeField] int health10;
    [SerializeField] int health5;
    [SerializeField] int health1;
    [SerializeField] int grenadeAmount;
    [Space]
    [SerializeField] private TextMeshProUGUI TextBTN1;
    [SerializeField] private TextMeshProUGUI TextBTN2;
    [SerializeField] private TextMeshProUGUI TextBTN3;
    [SerializeField] private TextMeshProUGUI TextBTN4;
    [Space]
    [SerializeField] private Button BTN1;
    [SerializeField] private Button BTN2;
    [SerializeField] private Button BTN3;
    [SerializeField] private Button BTN4;
    [Space]
    [SerializeField] HealthBar healthBar;
    [SerializeField] Money moneyScript;
    [SerializeField] GrenadeUIScript GrenadeUIScript;

    void Start()
    {
        EnableButtons();
        announcement = FindFirstObjectByType<announcement>();

        TextBTN1.text = $"{health10} HP: {price10hp}$";
        TextBTN2.text = $"{health5} HP: {price5hp}$";
        TextBTN3.text = $"{health1} HP: {price1hp}$";
        TextBTN4.text = $"{grenadeAmount} Grenades: {price1grenade}$";

        healthBar.setMaxHealth(PlayerStats.maxHp);
        healthBar.SetHealth(PlayerStats.hp);
        moneyScript.setNumber(PlayerStats.money);
        GrenadeUIScript.setGrenades(PlayerStats.grenades);
        PlayerStats.gameHasStarted = true;
    }

    public void add10hp()
    {

        if (PlayerStats.money < price10hp)
        {
            notEnoughFunds(price10hp);
        }

        if (PlayerStats.hp + health10 > PlayerStats.maxHp && PlayerStats.money > price10hp)
        {
            tooMuchHealth(health10);
        }
        //kollar så spelaren har tillräckligt med pengar och tillräckligt lite hp
        if (PlayerStats.money >= price10hp && PlayerStats.hp + health10 < PlayerStats.maxHp)
        {
            PlayerStats.hp = PlayerStats.hp + health10;
            PlayerStats.money -= price10hp;

            healthBar.SetHealth(PlayerStats.hp);
            moneyScript.setNumber(PlayerStats.money);
        }
    }

    public void add5hp()
    {

        if (PlayerStats.money < price5hp)
        {
            notEnoughFunds(price5hp);
        }

        if (PlayerStats.hp + health5 > PlayerStats.maxHp && PlayerStats.money > price5hp)
        {
            tooMuchHealth(health5);
        }

        //kollar så spelaren har tillräckligt med pengar och tillräckligt lite hp
        if (PlayerStats.money >= price5hp && PlayerStats.hp + health5 <= PlayerStats.maxHp)
        {
            PlayerStats.hp = PlayerStats.hp + health5;
            PlayerStats.money -= price5hp;

            healthBar.SetHealth(PlayerStats.hp);
            moneyScript.setNumber(PlayerStats.money);
        }
    }

    public void add1hp()
    {

        if (PlayerStats.money < price1hp)
        {
            notEnoughFunds(price1hp);
        }

        if (PlayerStats.hp + health1 > PlayerStats.maxHp && PlayerStats.money > price1hp)
        {
            tooMuchHealth(health1);
        }

        //kollar så spelaren har tillräckligt med pengar och tillräckligt lite hp
        if (PlayerStats.money >= price1hp && PlayerStats.hp + health1 <= PlayerStats.maxHp)
        {
            PlayerStats.hp = PlayerStats.hp + health1;
            PlayerStats.money -= price1hp;

            healthBar.SetHealth(PlayerStats.hp);
            moneyScript.setNumber(PlayerStats.money);
        }
    }

    public void addGrenade()
    {
        if (PlayerStats.money < price1grenade)
        {
            notEnoughFunds(price1grenade);
        }

        if (PlayerStats.money >= price1grenade)
        {
            PlayerStats.grenades = PlayerStats.grenades + grenadeAmount;
            PlayerStats.money -= price1grenade;

            moneyScript.setNumber(PlayerStats.money);
            GrenadeUIScript.setGrenades(PlayerStats.grenades);
        }
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

    private IEnumerator DisableButtonsWithDelay()
    {
        yield return new WaitForSeconds(1f);

        BTN1.interactable = false;
        BTN2.interactable = false;
        BTN3.interactable = false;
        BTN4.interactable = false;
    }

    public void DisableButtons()
    {
        StartCoroutine(DisableButtonsWithDelay());
    }

    private void EnableButtons()
    {
        BTN1.interactable = true;
        BTN2.interactable = true;
        BTN3.interactable = true;
        BTN4.interactable = true;
    }
}


