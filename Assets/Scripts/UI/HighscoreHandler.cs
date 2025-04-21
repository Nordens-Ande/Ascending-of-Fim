using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class HighscoreHandler : MonoBehaviour
{

    private string filePath;
    private List<int> highScores = new List<int>();
    private int maxSavedScores = 5;

    private void Awake()
    {
        filePath = Path.Combine(Application.persistentDataPath, "highscores.txt");
        LoadHighScores();
    }

    public void LoadHighScores()
    {
        
        highScores.Clear();

        if (File.Exists(filePath))
        {
            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines)
            {
                if (int.TryParse(line.Trim(), out int score))
                {
                    highScores.Add(score);
                }
            }

            highScores.Sort((a, b) => b.CompareTo(a));
        }
        else
        {
            Debug.Log("No highscore file found, starting fresh." + filePath);
            //gör den nya highscore filen och stänger streamreadern
            File.Create(filePath).Close();
        }

    }


    public void AddNewScore(int score)
    {
        highScores.Add(score);
        highScores.Sort((a, b) => b.CompareTo(a));

        if(highScores.Count > maxSavedScores)
        {
            //keep onlt the top scores
            highScores = highScores.GetRange(0, maxSavedScores);
        }
        SaveHighScores();
    }

    public void SaveHighScores()
    {
        //gör om alla ints till string och sparar i textfilen
        File.WriteAllLines(filePath, highScores.ConvertAll(score => score.ToString()));
        Debug.Log("highscores saved");
    }

    public List<int> GetHighScores()
    {
        return new List<int>(highScores);
    }
 
}
