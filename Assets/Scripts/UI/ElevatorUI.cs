using TMPro;
using UnityEngine;

public class ElevatorUI : MonoBehaviour
{
    
    private HUDHandler hudHandler;

    [SerializeField] int price10;
    [SerializeField] int price5;
    [SerializeField] int price1;

    [SerializeField] private TextMeshProUGUI text10;
    [SerializeField] private TextMeshProUGUI text5;
    [SerializeField] private TextMeshProUGUI text1;

    [SerializeField] HealthBar healthBar;
    [SerializeField] Money moneyScript;

    void Start()
    {
        //hudHandler = GetComponent<HUDHandler>();

        text10.text = price10.ToString();
        text5.text = price5.ToString();
        text1.text = price1.ToString();

        //PlayerStats.hp = 30;
        //PlayerStats.money = 10000;

        healthBar.setMaxHealth(PlayerStats.maxHp);
        healthBar.SetHealth(PlayerStats.hp);
        moneyScript.setNumber(PlayerStats.money);
    }

    public void add10hp()
    {
        //kollar så spelaren har tillräckligt med pengar och tillräckligt lite hp
        if(PlayerStats.money >= price10 && PlayerStats.hp + 10 <= PlayerStats.maxHp)
        {
            PlayerStats.hp = PlayerStats.hp + 10;
            PlayerStats.money -= price10;

            healthBar.SetHealth(PlayerStats.hp);
            moneyScript.setNumber(PlayerStats.money);
        }
    }

    public void add5hp()
    {
        //kollar så spelaren har tillräckligt med pengar och tillräckligt lite hp
        if (PlayerStats.money >= price5 && PlayerStats.hp + 5 <= PlayerStats.maxHp)
        {
            PlayerStats.hp = PlayerStats.hp + 5;
            PlayerStats.money -= price5;

            healthBar.SetHealth(PlayerStats.hp);
            moneyScript.setNumber(PlayerStats.money);
        }
    }

    public void add1hp()
    {
        //kollar så spelaren har tillräckligt med pengar och tillräckligt lite hp
        if (PlayerStats.money >= price1 && PlayerStats.hp + 1 <= PlayerStats.maxHp)
        {
            PlayerStats.hp = PlayerStats.hp + 1;
            PlayerStats.money -= price1;

            healthBar.SetHealth(PlayerStats.hp);
            moneyScript.setNumber(PlayerStats.money);
        }
    }
}
