using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] EnemyManager enemyManager;
    [SerializeField] NavMeshBaker navMeshBaker;
    [SerializeField] RoomManager roomManager;
    [SerializeField] SceneHandler sceneHandler;

    private HUDHandler hudHandler;
    private PlayerHealth playerHealth;

    enum GameState { MainMenu, Playing, Elevator, GameOver}
    GameState gameState;

    bool hasKeycard;

    void Start()
    {
        hudHandler = FindFirstObjectByType<HUDHandler>();
        playerHealth = FindFirstObjectByType<PlayerHealth>();

        gameState = GameState.MainMenu;
        hasKeycard = false;
    }

    public void PlayerFoundKeycard() // call from player interact script
    {
        hasKeycard = true;
        hudHandler?.hasKeycard();
    }

    public void LoadLevel() //KOPPLAD!
    {
        hudHandler?.DontHaveKeycard();
        PlayerStats.currentLevel += 1;

        roomManager.reroll = true; // reroll the apartment
        roomManager.floorLevel += 1; // increase the floor level

        navMeshBaker.StartCoroutine(navMeshBaker.BakeNavMesh());
        
    }

    public void RestartGame()//KOPPLAD!
    {
        if (PlayerStats.gameHasStarted)
        {
            sceneHandler.RestartScene();
            PlayerStats.resetValues();
        }
    }

    public void StartLevel() // start enemy spawning, playing input etc, maybe reload weapon
    {
        hudHandler?.DontHaveKeycard();
    }

    public void FinishedLevel() // enters elevator
    {
        //change scene? till hissen
        hasKeycard = false;
    }

    public void StopLevel() // stop enemy spawning, player input etc
    {
       
    }

    public void GameOver() // player dies, save highscore, go to game over screen or main menu, etc
    {
        gameState = GameState.GameOver;
        hasKeycard = false;
    }
}
