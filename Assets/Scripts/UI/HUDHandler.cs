using UnityEngine;

public class HUDHandler : MonoBehaviour
{
    private TimerUIScript timerUIScript;
    private Money moneyScript;
    private HealthBar healthBarScript;
    private Ammo ammoScript;
    private announcement announcementScript;
    private GameOverScore gameOverScore;
    private HighscoreHandler highscoreHandler;
    private ScoreScript scoreScript;


    private void Start()
    {
        timerUIScript = GetComponent<TimerUIScript>();
        moneyScript = GetComponent<Money>();
        healthBarScript = GetComponent<HealthBar>();
        ammoScript = GetComponent<Ammo>();
        announcementScript = GetComponent<announcement>();
        gameOverScore = GetComponent<GameOverScore>();
        highscoreHandler = GetComponent<HighscoreHandler>();
        scoreScript = GetComponent<ScoreScript>();
    }

    //timer
    public void stopTimer()
    {
        timerUIScript.StopTimer();
    }

    public void startTimer()
    {
        timerUIScript.StartTimer();
    }

    public void resetTimer()
    {
        timerUIScript.ResetTimer();
    }


    //money
    public void setMoney(int newMoney)
    {
        moneyScript.setNumber(newMoney);
    }

    public void addMoney(int addMoney)
    {
        moneyScript.addMoney(addMoney);
    }

    public void subtractMoney(int subtractMoney)
    {
        moneyScript.subtractMoney(subtractMoney);
    }


    //healthbar
    public void setHealth(float health)
    {
        healthBarScript.SetHealth(health);
    }

    public void addHealth(float addHealth)
    {
        healthBarScript.AddHealth(addHealth);
    }

    public void subtractHealth(float subtractHealth)
    {
        healthBarScript.SubtractHealth(subtractHealth);
    }


    //ammo
    public void setAmmo(int ammo)
    {
        ammoScript.setAmmo(ammo);
    }

    public void addAmmo(int addAmmo)
    {
        ammoScript.addAmmo(addAmmo);
    }

    public void subtractAmmo(int subtractAmmo)
    {
        ammoScript.subtractAmmo(subtractAmmo);
    }

    //annoucnment
    public void setAnnounchment(string text, int time)
    {
        announcementScript.SetAnnouncementText(text, time);
    }

    //gameover
    public void setGameoverScore(int score)
    {
        gameOverScore.setGameOverScore(score);
    }

    //highscore
    public void addNewHighscore(int score)
    {
        highscoreHandler.AddNewScore(score);
    }

    //score for the hud
    public void setScore(int newScore)
    {
        scoreScript.setScore(newScore);
    }

    public void addScore(int amount)
    {
        scoreScript.addScore(amount);
    }

    public void subtractScore(int amount)
    {
        scoreScript.subtractScore(amount);
    }


}
