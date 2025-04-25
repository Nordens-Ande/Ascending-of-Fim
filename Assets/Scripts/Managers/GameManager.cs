using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] EnemyManager enemyManager;
    [SerializeField] NavMeshBaker navMeshBaker;

    enum GameState { MainMenu, Playing, Elevator, GameOver}
    GameState gameState;

    public int level { get; private set; }

    bool hasKeycard;

    void Start()
    {
        gameState = GameState.MainMenu;
        level = 0;
        hasKeycard = false;
    }

    public void PlayerFoundKeycard() // call from player interact script
    {
        hasKeycard = true;
    }

    public void LoadLevel() // load a new level
    {
        level += 1;
        //byt scene till playing
        //generate apartment
        navMeshBaker.StartCoroutine(navMeshBaker.BakeNavMesh());
        //reset player position
    }

    public void StartLevel() // start enemy spawning, playing input etc, maybe reload weapon
    {
       
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
