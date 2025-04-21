using TMPro;
using UnityEngine;

public class GameOverScore : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI gameOverScore;

    public void setGameOverScore(int newMoney)
    {
        if (gameOverScore != null)
        {
            gameOverScore.text = newMoney.ToString();
        }
        else
        {
            Debug.LogWarning("TextMeshProUGUI reference is not assigned");
        }
    }
}
