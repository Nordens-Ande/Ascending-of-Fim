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
        Instantiate(enemyPrefab, GetRandomSpawnPoint(), Quaternion.identity);
    }

    Vector3 GetRandomSpawnPoint()
    {
        List<Vector3> viableSpawnPoints = spawnPointManager.GetViableSpawnPoints();
        Debug.Log(viableSpawnPoints.Count);
        int random = Random.Range(0, viableSpawnPoints.Count);
        Vector3 spawnPointPos = viableSpawnPoints[random];
        return spawnPointPos;
    }

    void Update()
    {
        
    }
}
