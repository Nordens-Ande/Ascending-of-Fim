using TMPro;
using UnityEngine;

public class LevelIndicatorUIScript : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI levelText;


    void Start()
    {
        if (levelText != null)
        {
            levelText.text = PlayerStats.currentLevel.ToString();
        }
        else
        {
            Debug.LogWarning("TextMeshProUGUI reference is not assigned for the level indcator in UI");
        }
    }

    public void setNumber(int newLevel)
    {
        if (levelText != null)
        {
            levelText.text = newLevel.ToString();
        }
        else
        {
            Debug.LogWarning("TextMeshProUGUI reference is not assigned for the level indcator in UI");
        }
    }

    public void addLevel(int amountToAdd)
    {
        if (levelText != null)
        {
            PlayerStats.currentLevel += amountToAdd;
            setNumber(PlayerStats.currentLevel);

        }
        else
        {
            Debug.LogWarning("TextMeshProUGUI reference is not assigned for the level indcator in UI");
        }
    }

    public void subtractLevel(int amountToSubtract)//idk when this would be used, but it's here
    {
        if (levelText != null)
        {
            PlayerStats.currentLevel -= amountToSubtract;
            setNumber(PlayerStats.currentLevel);
        }
        else
        {
            Debug.LogWarning("TextMeshProUGUI reference is not assigned for the level indcator in UI");
        }
    }
}
