using TMPro;
using UnityEngine;

public class GameOverScore : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI gameOverScore;
    HighscoreHandler HighscoreHandler;
    private HUDHandler hudHandler;

    private void Start()
    {
        HighscoreHandler = FindAnyObjectByType<HighscoreHandler>();
        hudHandler = FindFirstObjectByType<HUDHandler>();

        if (gameOverScore != null && PlayerStats.playerHasDied)
        {
            gameOverScore.text = PlayerStats.score.ToString();
            HighscoreHandler.AddNewScore(PlayerStats.score);
            PlayerStats.score = 0;
            hudHandler.playerisDead();
        }
        else
        {
            Debug.LogWarning("TextMeshProUGUI reference is not assigned");
        }
    }

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
