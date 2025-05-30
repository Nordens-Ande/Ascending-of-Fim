using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] EnemyManager enemyManager;
    [SerializeField] NavMeshBaker navMeshBaker;
    [SerializeField] RoomManager roomManager;
    [SerializeField] BackgroundHandler backgroundHandler;
    [SerializeField] SceneHandler sceneHandler;
    [SerializeField] HUDHandler hudHandler;

    //private HUDHandler hudHandler;
    private PlayerHealth playerHealth;

    enum GameState { MainMenu, Playing, Elevator, GameOver}
    GameState gameState;

    bool hasKeycard;
    public bool HasKeycard { get { return hasKeycard; } }

    //VALUES FOR RESTART
    MinMaxInt roomAmountRangeInit;

    void Start()
    {
        //hudHandler = FindFirstObjectByType<HUDHandler>();
        playerHealth = FindFirstObjectByType<PlayerHealth>();

        gameState = GameState.MainMenu;
        hasKeycard = false;

        //VALUES FOR RESTART
        MinMaxInt roomAmountRangeInit = roomManager.roomAmountRange;
    }

    public void PlayerFoundKeycard() // call from player interact script
    {
        hasKeycard = true;
        hudHandler?.hasKeycard();
        hudHandler?.setAnnounchment("Keycard found!", 2);
    }

    public void LoadLevel() //KOPPLAD!
    {
        hudHandler?.DontHaveKeycard();
        hasKeycard = false;
        PlayerStats.currentLevel = PlayerStats.currentLevel + 1;
        Debug.LogWarning(PlayerStats.currentLevel + "level");
        hudHandler.setLevel(PlayerStats.currentLevel);
        PlayerStats.elapsedTimePerLevel = 0f;

        roomManager.floorLevel += 1; // increase the floor level
        roomManager.roomAmountRange = new MinMaxInt(roomManager.roomAmountRange.min + 1, roomManager.roomAmountRange.max + 2);
        roomManager.reroll = true; // reroll the apartment

        backgroundHandler.currentFloor = roomManager.floorLevel;

        navMeshBaker.StartCoroutine(navMeshBaker.BakeNavMesh());
        
    }

    public void RestartGame()//KOPPLAD!
    {
        if (PlayerStats.gameHasStarted)
        {
            roomManager.floorLevel = 1;
            roomManager.roomAmountRange = roomAmountRangeInit;
            backgroundHandler.currentFloor = roomManager.floorLevel;

            sceneHandler.RestartScene();
            PlayerStats.resetValues();
        }
    }

    //public void StartLevel() // start enemy spawning, playing input etc, maybe reload weapon
    //{
    //    hudHandler?.DontHaveKeycard();
    //}

    //public void FinishedLevel() // enters elevator
    //{
    //    //change scene? till hissen
    //    hasKeycard = false;
    //}

    //public void StopLevel() // stop enemy spawning, player input etc
    //{

    //}

    //public void GameOver() // player dies, save highscore, go to game over screen or main menu, etc
    //{
    //    gameState = GameState.GameOver;
    //    hasKeycard = false;
    //}
}
