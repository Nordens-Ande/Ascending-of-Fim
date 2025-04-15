using TMPro;
using UnityEngine;

public class GameOverScore : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI gameOverScore;

    public void setGameOverScore(int score)
    {
        if (gameOverScore != null)
        {
            gameOverScore.text = score.ToString();
        }
        else
        {
            Debug.LogWarning("TextMeshProUGUI reference is not assigned");
        }
    }
}
