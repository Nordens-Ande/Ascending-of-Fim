using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemySpawnPointManager : MonoBehaviour
{
    GameObject player;
    List<Vector3> spawnPoints;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    public void GetSpawnPoints()
    {
        spawnPoints = new List<Vector3>();
        GameObject[] points = GameObject.FindGameObjectsWithTag("EnemySpawn");
        foreach(GameObject spawnPoint in points)
        {
            spawnPoints.Add(spawnPoint.transform.position);
        }
        Debug.Log(spawnPoints.Count + "spawnpoints found");
    }

    public List<Vector3> GetViableSpawnPoints()
    {
        List<Vector3> viableSpawns = new List<Vector3>();
        foreach(Vector3 spawnPoint in spawnPoints)
        {
            int minDistance = 10;
            float distance = Vector3.Distance(player.transform.position, spawnPoint);
            if(distance > minDistance)
            {
                viableSpawns.Add(spawnPoint);
            }
        }
        return viableSpawns;
    }

    void Update()
    {
        
    }
}
