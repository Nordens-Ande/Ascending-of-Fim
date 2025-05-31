using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyManager : MonoBehaviour
{
    List<GameObject> enemies;
    [SerializeField] GameObject enemyPrefab;

    [SerializeField] EnemySpawnPointManager spawnPointManager;

    int amountToSpawnAtStart = 4;

    float timeBeforeFirstEnemySpawn = 9;
    float timeBetweenEnemySpawns = 4;

    void Start()
    {
        StartCoroutine(SpawnEnemies()); // spawns enemies at start of level
        StartCoroutine(EnemySpawner(timeBeforeFirstEnemySpawn));
    }

    IEnumerator SpawnEnemies()
    {
        yield return new WaitForSeconds(2);
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
        bool check = ValidateSpawnPos(spawnPos).Item1;
        Vector3 validatedSpawnPos = ValidateSpawnPos(spawnPos).Item2;

        if(check)
            Instantiate(enemyPrefab, validatedSpawnPos, Quaternion.identity);
    }

    (bool, Vector3) ValidateSpawnPos(Vector3 spawnPos)
    {
        NavMeshHit hit;
        if(NavMesh.SamplePosition(spawnPos, out hit, 1, NavMesh.AllAreas))
        {
            return (true, hit.position);
        }
        else
        {
            return (false, Vector3.zero);
        }
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
            Vector3 spawnPointPos = new Vector3(2, 1.5f, 3); // safe spot in front of elevator incase no spawnpoints where found
            return spawnPointPos;
        }
    }
}
