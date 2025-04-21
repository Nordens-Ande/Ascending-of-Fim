using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class Money : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneytext;

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
            int currentMoney = int.Parse(moneytext.text);
            currentMoney = currentMoney + amountToAdd;
            setNumber(currentMoney);
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
            int currentMoney = int.Parse(moneytext.text);
            currentMoney = currentMoney - amountToAdd;
            setNumber(currentMoney);
        }
        else
        {
            Debug.LogWarning("TextMeshProUGUI reference is not assigned");
        }
    }

}
