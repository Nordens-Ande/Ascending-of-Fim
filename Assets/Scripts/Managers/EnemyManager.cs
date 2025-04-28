using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    List<GameObject> enemies;
    [SerializeField] GameObject enemyPrefab;

    [SerializeField] EnemySpawnPointManager spawnPointManager;

    int amountToSpawnAtStart = 7;

    float timeBeforeFirstEnemySpawn = 15;
    float timeBetweenEnemySpawns = 7;

    void Start()
    {
        StartCoroutine(SpawnEnemies()); // spawns enemies at start of level
        StartCoroutine(EnemySpawner(timeBeforeFirstEnemySpawn));
    }

    IEnumerator SpawnEnemies()
    {
        yield return new WaitForSeconds(0);
        Debug.Log("hi");
        foreach (Vector3 spawnPos in GetRandomUniqueSpawnPoints(amountToSpawnAtStart))
        {
            CreateEnemy(spawnPos);
        }
    }

    IEnumerator EnemySpawner(float timer)
    {
        yield return new WaitForSeconds(timer);
        CreateEnemy(GetRandomSpawnPoint());
        StartCoroutine(EnemySpawner(timeBetweenEnemySpawns));
    }

    void CreateEnemy(Vector3 spawnPos)
    {
        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
    }

    List<Vector3> GetRandomUniqueSpawnPoints(int amount)
    {
        List<Vector3> viableSpawnPoints = spawnPointManager.GetViableSpawnPoints(); // removes any spawnpoints too close to player spawn
        List<Vector3> usedSpawnPoints = new List<Vector3>();

        if(amount > viableSpawnPoints.Count)
        {
            amount = viableSpawnPoints.Count;
        }

        for(int i = 0; i < amount; i++)
        {
            bool foundUnique = false; // need to make sure not more than 1 enemy spawn at a spawnpoint
            while(foundUnique == false)
            {
                int random = Random.Range(0, viableSpawnPoints.Count);
                Vector3 spawnPoint = viableSpawnPoints[random];
                if(!usedSpawnPoints.Contains(spawnPoint))
                {
                    foundUnique = true;
                    usedSpawnPoints.Add(spawnPoint);
                }
                if(foundUnique == false)
                {
                    Debug.Log("failed to find spawnpoint");
                }
                
            }
        }
        return usedSpawnPoints;
    }

    Vector3 GetRandomSpawnPoint()
    {
        List<Vector3> viableSpawnPoints = spawnPointManager.GetViableSpawnPoints();
        if(viableSpawnPoints.Count > 0)
        {
            int random = Random.Range(0, viableSpawnPoints.Count);
            Vector3 spawnPointPos = viableSpawnPoints[random];
            return spawnPointPos;
        }
        else
        {
            return Vector3.zero;
        }
    }
}
