using TMPro;
using UnityEngine;

public class ScoreScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;


    private void Start()
    {
        if (scoreText != null)
        {
            scoreText.text = "0";
        }
        else
        {
            Debug.LogWarning("TextMeshProUGUI reference is not assigned");
        }
    }

    public void setScore(int newScore)
    {
        if (scoreText != null)
        {
            scoreText.text = newScore.ToString();
        }
        else
        {
            Debug.LogWarning("TextMeshProUGUI reference is not assigned");
        }
    }

    public void addScore(int amountToAdd)
    {
        if (scoreText != null)
        {
            int currentMoney = int.Parse(scoreText.text);
            currentMoney = currentMoney + amountToAdd;
            setScore(currentMoney);
        }
        else
        {
            Debug.LogWarning("TextMeshProUGUI reference is not assigned");
        }
    }

    public void subtractScore(int amountToAdd)
    {
        if (scoreText != null)
        {
            int currentMoney = int.Parse(scoreText.text);
            currentMoney = currentMoney - amountToAdd;
            setScore(currentMoney);
        }
        else
        {
            Debug.LogWarning("TextMeshProUGUI reference is not assigned");
        }
    }
}
