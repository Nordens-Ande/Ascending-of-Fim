using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class RoomPrefabManager : MonoBehaviour
{
    [SerializeField] GameObject roomObject;
    [SerializeField] List<GameObject> rooms;

    private Dictionary<GameObject, RoomType> roomTypes;
    private Dictionary<GameObject, Vector2Int> roomSizes;

    private Dictionary<GameObject, List<GameObject>> furnitures;
    private Dictionary<GameObject, List<Transform>> doors;
    private Dictionary<GameObject, List<Transform>> enemySpawns;
    private Dictionary<GameObject, List<Transform>> keycardSpawns;


    void Start()
    {
        
    }

    private Match NameParse()
    {
        string objectName = "Hallway_6x6";
        Regex regex = new Regex(@"^(?<type>[A-Za-z]+)_(?<width>\d+)x(?<height>\d+)$");

        Match match = regex.Match(objectName);
        if (match.Success)
        {
            string typeString = match.Groups["type"].Value;       //Namnet, dvs RoomType
            int width = int.Parse(match.Groups["width"].Value);   //Width
            int height = int.Parse(match.Groups["height"].Value); //Height

            Debug.Log($"Type: {typeString}, Width: {width}, Height: {height}");
            return match;
        }

        return null;
    }

    private void RoomParse()
    {

    }

    public void GetRoom(RoomType type, int width, int height, List<Vector2> doors)
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
