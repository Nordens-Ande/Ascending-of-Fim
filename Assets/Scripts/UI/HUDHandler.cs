using UnityEngine;
using FirstGearGames.SmoothCameraShaker;

public class HUDHandler : MonoBehaviour
{
    [SerializeField] UIHandler UIHandler;

    private TimerUIScript timerUIScript;
    private Money moneyScript;
    private HealthBar healthBarScript;
    private Ammo ammoScript;
    private announcement announcementScript;
    private GameOverScore gameOverScore;
    private HighscoreHandler highscoreHandler;
    private ScoreScript scoreScript;
    private HitUI hitUIscript;
    private KeycardUIScript keycardUI;

    [SerializeField] ShakeData hitShake;
    [SerializeField] ShakeData shootShake;

    //[SerializeField] TimerUIScript timerUIScript;
    //[SerializeField] Money moneyScript;
    //[SerializeField] HealthBar healthBarScript;
    //[SerializeField] Ammo ammoScript;
    //[SerializeField] announcement announcementScript;
    //[SerializeField] GameOverScore gameOverScore;
    //[SerializeField] HighscoreHandler highscoreHandler;
    //[SerializeField] ScoreScript scoreScript;
    //[SerializeField] HitUI hitUIscript;


    private void Awake()
    {
        timerUIScript = GetComponentInChildren<TimerUIScript>();
        moneyScript = GetComponentInChildren<Money>();
        healthBarScript = GetComponentInChildren<HealthBar>();
        ammoScript = GetComponentInChildren<Ammo>();
        announcementScript = GetComponentInChildren<announcement>();
        gameOverScore = GetComponentInChildren<GameOverScore>();
        highscoreHandler = GetComponentInChildren<HighscoreHandler>();
        scoreScript = GetComponentInChildren<ScoreScript>();
        hitUIscript = GetComponentInChildren<HitUI>();
        keycardUI = GetComponentInChildren<KeycardUIScript>();

        WarnIfNull(timerUIScript, nameof(timerUIScript));
        WarnIfNull(moneyScript, nameof(moneyScript));
        WarnIfNull(healthBarScript, nameof(healthBarScript));
        WarnIfNull(ammoScript, nameof(ammoScript));
        WarnIfNull(announcementScript, nameof(announcementScript));
        WarnIfNull(gameOverScore, nameof(gameOverScore));
        WarnIfNull(highscoreHandler, nameof(highscoreHandler));
        WarnIfNull(scoreScript, nameof(scoreScript));
        WarnIfNull(hitUIscript, nameof(hitUIscript));
        WarnIfNull(keycardUI, nameof(keycardUI));
    }


    private void WarnIfNull(Object obj, string name)
    {
        if (obj == null)
        {
            Debug.LogWarning($"{name} is missing from HUD hierarchy!", this);
        }
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

    public float getTime()
    {
        return timerUIScript.getTime();
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

    public int getMoney()
    {
        return moneyScript.readMoney();
    }

    //healthbar
    public void setHealth(float health)
    {
        healthBarScript.SetHealth(health);
    }

    public void setMaxHealth(float health)
    {
        UIHandler.ToggleHUD();
        healthBarScript.setMaxHealth(health);
        UIHandler.ToggleHUD();
    }

    public void addHealth(float addHealth)
    {
        healthBarScript.AddHealth(addHealth);
    }

    public void subtractHealth(float subtractHealth)
    {
        healthBarScript.SubtractHealth(subtractHealth);
    }

    public float getHealth()
    {
        return healthBarScript.readHealth();
    }

    public float getMaxHealth()
    {
        return healthBarScript.getMaxHealth();
    }

    //hit visual
    public void hitVisualUI(float duration)
    {
        hitUIscript.Show(duration);
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


    //keycard
    public void hasKeycard()
    {
        keycardUI.playerHasKeycard();
    }

    public void DontHaveKeycard()
    {
        keycardUI.playerDoNotHaveKeycard();
    }


    //camera shake
    public void FimShotShake()
    {
        CameraShakerHandler.Shake(hitShake);
    }

    public void FimShootingShake()
    {
        CameraShakerHandler.Shake(shootShake);
    }

    //menu bool
    public bool isMenuActive()
    {
        return UIHandler.isMenuActive();
    }
}
