using TMPro;
using UnityEngine;

public class ScoreScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;


    private void Start()
    {
        if (scoreText != null)
        {
            scoreText.text = PlayerStats.score.ToString();
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
            PlayerStats.score = newScore;
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
            int currentScore = int.Parse(scoreText.text);
            currentScore = currentScore + amountToAdd;
            setScore(currentScore);
            PlayerStats.score = currentScore;
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
            int currentScore = int.Parse(scoreText.text);
            currentScore = currentScore - amountToAdd;
            setScore(currentScore);
            PlayerStats.score = currentScore;
        }
        else
        {
            Debug.LogWarning("TextMeshProUGUI reference is not assigned");
        }
    }
}
