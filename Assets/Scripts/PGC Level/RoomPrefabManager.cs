using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class RoomPrefabManager : MonoBehaviour
{
    [SerializeField] GameObject roomObject;
    [SerializeField] List<Furniture> furnitures;


    void Start()
    {

    }

    public List<Furniture> GetFurniture(RoomType type, Vector2Int size)
    {
        List<Furniture> result = new List<Furniture>();

        foreach(Furniture furniture in furnitures)
        {
            if (furniture.roomType == type && (furniture.size == size || (furniture.size.y == size.x && furniture.size.x == size.y)))
            {
                result.Add(furniture);
            }
        }

        return result;
    }



    //private Match NameParse()
    //{
    //    string objectName = "Hallway_6x6";
    //    Regex regex = new Regex(@"^(?<type>[A-Za-z]+)_(?<width>\d+)x(?<height>\d+)$");

    //    Match match = regex.Match(objectName);
    //    if (match.Success)
    //    {
    //        string typeString = match.Groups["type"].Value;       //Namnet, dvs RoomType
    //        int width = int.Parse(match.Groups["width"].Value);   //Width
    //        int height = int.Parse(match.Groups["height"].Value); //Height

    //        Debug.Log($"Type: {typeString}, Width: {width}, Height: {height}");
    //        return match;
    //    }

    //    return null;
    //}

    //private void RoomParse()
    //{

    //}

    //public void GetRoom(RoomType type, int width, int height, List<Vector2> doors)
    //{

    //}

    // Update is called once per frame
    void Update()
    {
        
    }
}
