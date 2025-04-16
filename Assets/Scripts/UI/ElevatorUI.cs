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
        
    }

    public void add10hp()
    {

        hudHandler.addHealth(10f);
    }

    public void add5hp()
    {
        hudHandler.addHealth(5f);
    }

    public void add1hp()
    {
        hudHandler.addHealth(1f);
    }


    void Update()
    {
        
    }


    void ChangeScence()
    {

    }
}
