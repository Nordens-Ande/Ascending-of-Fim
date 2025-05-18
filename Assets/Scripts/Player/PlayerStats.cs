using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static int maxHp = 100; //max health
    public static int hp = 100; //current health
    public static int money = 0; //money in game
    public static bool gameHasStarted = false; //to autostart game again when you're moved back from elevator
    public static int currentLevel = 0; //current level
    public static int score = 0; //score
    public static bool playerHasDied = false; //if the player has died
    public static float elapsedTime = 0f; //time elapsed


    public static void resetValues()
    {
        maxHp = 100;
        hp = 100;
        money = 0;
        currentLevel = 0;
        score = 0;
        playerHasDied = false;
        elapsedTime = 0f;
    }


}

