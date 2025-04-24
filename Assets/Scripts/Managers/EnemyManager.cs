using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    List<GameObject> enemies;
    [SerializeField] GameObject enemyPrefab;

    [SerializeField] EnemySpawnPointManager spawnPointManager;

    void Start()
    {
        StartCoroutine(EnemySpawner());
    }

    IEnumerator EnemySpawner()
    {
        yield return new WaitForSeconds(5);
        CreateEnemy();
        StartCoroutine(EnemySpawner());
    }

    void CreateEnemy()
    {
        Vector3 spawnPoint = GetRandomSpawnPoint();
        if(spawnPoint != Vector3.zero)
        {
            Instantiate(enemyPrefab, spawnPoint, Quaternion.identity);
        }
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
