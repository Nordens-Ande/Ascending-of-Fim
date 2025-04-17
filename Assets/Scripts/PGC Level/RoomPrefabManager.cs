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


    public List<Furniture> GetFurniture(RoomType type)
    {
        List<Furniture> result = new List<Furniture>();

        foreach (Furniture furniture in furnitures)
        {
            if (furniture.roomType == type)
            {
                result.Add(furniture);
            }
        }

        return result;
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

    //public void Rotate(Furniture furniture)
    //{
    //    //Furniture newFurniture = GameObject.Instantiate(furniture);
    //    Vector2Int size = furniture.size;
    //    furniture.size = new Vector2Int(furniture.size.y, furniture.size.x);
    //    furniture.frontDirection = new Vector2Int(furniture.frontDirection.y, furniture.frontDirection.x);
    //    foreach (Vector2Int dir in furniture.wallDirections)
    //        furniture.wallDirections.Add(new Vector2Int(dir.y, dir.x));
    //    furniture.transform.rotation = Quaternion.Euler(0, -90, 0);
    //}
    //public Furniture FlipVectors(Furniture furniture)
    //{
    //    Furniture newFurniture = new Furniture();
    //    newFurniture.size = furniture.size * -1;
    //    newFurniture.frontDirection = furniture.frontDirection * -1;
    //    foreach (Vector2Int dir in furniture.wallDirections)
    //        newFurniture.wallDirections.Add(dir * -1);
    //    newFurniture.transform.rotation = Quaternion.Euler(0, -180, 0);
    //    return newFurniture;
    //}

    void Update()
    {
        
    }
}
