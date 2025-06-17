using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyManager : MonoBehaviour
{
    List<GameObject> enemies;
    [SerializeField] GameObject enemyPrefab;

    [SerializeField] EnemySpawnPointManager spawnPointManager;

    //time thresholds for enemy spawning logic
    int amountToSpawnAtStart = 4;

    float timeBeforeFirstEnemySpawn = 9;
    float timeBetweenEnemySpawns = 4;

    void Start()
    {
        StartCoroutine(SpawnEnemies()); // spawns enemies at start of level
        StartCoroutine(EnemySpawner(timeBeforeFirstEnemySpawn));
    }

    IEnumerator SpawnEnemies() //this enumerator spawns the set amount of enemies at the start of a level
    {
        yield return new WaitForSeconds(2);
        foreach (Vector3 spawnPos in GetRandomUniqueSpawnPoints(amountToSpawnAtStart))
        {
            CreateEnemy(spawnPos);
        }
    }

    IEnumerator EnemySpawner(float timer) // this enumerator spawns 1 enemy at a time interval based on the argument input
    {
        yield return new WaitForSeconds(timer);
        CreateEnemy(GetRandomSpawnPoint());
        StartCoroutine(EnemySpawner(timeBetweenEnemySpawns));
    }

    void CreateEnemy(Vector3 spawnPos) // method to check the validation of the spawnpoint and then instantiate the enemy gameobject
    {
        bool check = ValidateSpawnPos(spawnPos).Item1;
        Vector3 validatedSpawnPos = ValidateSpawnPos(spawnPos).Item2;

        if(check)
            Instantiate(enemyPrefab, validatedSpawnPos, Quaternion.identity);
    }

    (bool, Vector3) ValidateSpawnPos(Vector3 spawnPos) //check if the spawnpoint is on the navmesh, added this method towards the end of the
                                                       //project to fix a bug with enemies spawning outside of the map
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

    List<Vector3> GetRandomUniqueSpawnPoints(int amount) //returns multiple spawnpoints, ensuring each of those spawn points are unique so not multiple enemies
                                                         //spawn at the same spawnpoint, this is used when spawning multiple enemies at the start of a level.
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

    Vector3 GetRandomSpawnPoint() // returns a single random spawnpoint used later in the level when only spawning 1 enemy at a time interval
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
