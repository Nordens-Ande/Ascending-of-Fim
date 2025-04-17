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

    void Start()
    {
        hudHandler = GetComponent<HUDHandler>();

        text10.text = price10.ToString();
        text5.text = price5.ToString();
        text1.text = price1.ToString();
    }

    public void add10hp()
    {
        //kollar så spelaren har tillräckligt med pengar och tillräckligt lite hp
        if(hudHandler.getMoney() >= price10 && hudHandler.getMaxHealth() - hudHandler.getHealth() >= 10f)
        {
            hudHandler.addHealth(10f);
            hudHandler.subtractMoney(price10);
        }
    }

    public void add5hp()
    {
        //kollar så spelaren har tillräckligt med pengar och tillräckligt lite hp
        if (hudHandler.getMoney() >= price5 && hudHandler.getMaxHealth() - hudHandler.getHealth() >= 5f)
        {
            hudHandler.addHealth(5f);
            hudHandler.subtractMoney(price5);
        }
    }

    public void add1hp()
    {
        //kollar så spelaren har tillräckligt med pengar och tillräckligt lite hp
        if (hudHandler.getMoney() >= price1 && hudHandler.getMaxHealth() - hudHandler.getHealth() >= 1f)
        {
            hudHandler.addHealth(1f);
            hudHandler.subtractMoney(price1);
        }
    }


    void ChangeScence()
    {

    }
}
