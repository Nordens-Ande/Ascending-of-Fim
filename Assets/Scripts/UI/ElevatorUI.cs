using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ElevatorUI : MonoBehaviour
{
    private announcement announcement;

    // Prices for the different options in the elevator
    [SerializeField] int price10hp;
    [SerializeField] int price5hp;
    [SerializeField] int price1hp;
    [SerializeField] int price1grenade;
    [Space] // The amount you get by purchasing
    [SerializeField] int health10;
    [SerializeField] int health5;
    [SerializeField] int health1;
    [SerializeField] int grenadeAmount;
    [Space] // The text on the corresponding buttons
    [SerializeField] private TextMeshProUGUI TextBTN1;
    [SerializeField] private TextMeshProUGUI TextBTN2;
    [SerializeField] private TextMeshProUGUI TextBTN3;
    [SerializeField] private TextMeshProUGUI TextBTN4;
    [Space] // The buttons
    [SerializeField] private Button BTN1;
    [SerializeField] private Button BTN2;
    [SerializeField] private Button BTN3;
    [SerializeField] private Button BTN4;
    [Space] // Scripts needed to interact with these buttons
    [SerializeField] HealthBar healthBar;
    [SerializeField] Money moneyScript;
    [SerializeField] GrenadeUIScript GrenadeUIScript;
    [SerializeField] GameObject countDown;
    [SerializeField] GameObject healthbar;
    [SerializeField] GameObject money;
    [SerializeField] GameObject annoucnement;
    [SerializeField] GameObject grenadeUI;

    void Start()
    {
        EnableButtons(); // For a bug where game crashed when you tried to buy stuff at scene change
        activateAllHUDObjects();
        announcement = FindFirstObjectByType<announcement>();

        TextBTN1.text = $"{health10} HP: {price10hp}$";
        TextBTN2.text = $"{health5} HP: {price5hp}$";
        TextBTN3.text = $"{health1} HP: {price1hp}$";
        TextBTN4.text = $"{grenadeAmount} Grenades: {price1grenade}$";

        // Updating visual values in the new scene
        healthBar.setMaxHealth(PlayerStats.maxHp);
        healthBar.SetHealth(PlayerStats.hp);
        moneyScript.setNumber(PlayerStats.money);
        GrenadeUIScript.setGrenades(PlayerStats.grenades);
        PlayerStats.gameHasStarted = true;
    }

    public void add10hp() // The first hp button that gives the most
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

    public void add5hp() // Second hp button with less hp but better value
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

    public void add1hp() // Last hp button with even less hp but best value
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

    public void addGrenade() // Button to buy grenades
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


    private void notEnoughFunds(int price) // This method is called if the player doesn't have the funds required.
    {
        if (PlayerStats.money < price)
        {
            announcement.SetAnnouncementText($"Not enough funds", 2f);
        }
    }

    private void tooMuchHealth(int health) // This method is called if the player is trying to purchase more hp than max
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

    public void DisableButtons() // Disable buttons as a bugfix for scene crashing
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

    private void activateAllHUDObjects()
    {
        countDown.SetActive(true);
        healthbar.SetActive(true);
        money.SetActive(true);
        annoucnement.SetActive(true);
        grenadeUI.SetActive(true);
    }
}


