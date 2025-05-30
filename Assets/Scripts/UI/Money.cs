using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class Money : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneytext;
    //private int money;


    private void Start()
    {
        if (moneytext != null)
        {
            //money = 0;
            moneytext.text = PlayerStats.money.ToString();
        }
        else
        {
            Debug.LogWarning("TextMeshProUGUI reference is not assigned");
        }
    }

    public int readMoney()
    {
        return PlayerStats.money;

        //if(moneytext != null)
        //{
        //    return int.Parse(moneytext.text);

        //}
        //else
        //{
        //    Debug.LogWarning("money is null");
        //    return 0;
        //}
    }

    public void setNumber(int newMoney)
    {
        if (moneytext != null)
        {
            moneytext.text = newMoney.ToString();
        }
        else
        {
            Debug.LogWarning("TextMeshProUGUI reference is not assigned");
        }
    }

    public void addMoney(int amountToAdd)
    {
        if(moneytext != null)
        {
            PlayerStats.money += amountToAdd;
            setNumber(PlayerStats.money);
            
        }
        else
        {
            Debug.LogWarning("TextMeshProUGUI reference is not assigned");
        }
    }

    public void subtractMoney(int amountToAdd)
    {
        if (moneytext != null)
        {
            PlayerStats.money -= amountToAdd;
            setNumber(PlayerStats.money);
        }
        else
        {
            Debug.LogWarning("TextMeshProUGUI reference is not assigned");
        }
    }

}
