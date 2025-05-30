using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayHighscore : MonoBehaviour
{

    [SerializeField] HighscoreHandler highscoreHandler;
    [SerializeField] private TextMeshProUGUI highScoreText;
   

    void Start()
    {
        DisplayHighScores();
    }

    public void DisplayHighScores()
    {
        if (highscoreHandler != null)
        {
            List<int> scores = highscoreHandler.GetHighScores();

            highScoreText.text = "Highscores:\n";

            for (int i = 0; i < scores.Count; i++)
            {
                highScoreText.text += $"{i + 1}. {scores[i]}\n";
            }
        }
    }
}
