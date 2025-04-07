using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemySpawnPointManager : MonoBehaviour
{
    GameObject player;

    void Start()
    {
        //create spawnPoints
        
        player = GameObject.Find("Player Equip");
    }

    public List<Vector3> GetViableSpawnPoints()
    {
        List<Vector3> viableSpawns = new List<Vector3>();
        foreach(Transform spawnPoint in transform.GetComponentsInChildren<Transform>())
        {
            int minDistance = 8;
            float distance = Vector3.Distance(player.transform.position, spawnPoint.transform.position);
            if(distance > minDistance)
            {
                viableSpawns.Add(spawnPoint.position);
            }
        }
        return viableSpawns;
    }

    void Update()
    {
        
    }
}
