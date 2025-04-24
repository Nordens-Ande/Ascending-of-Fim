using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class RoomPrefabManager : MonoBehaviour
{
    [SerializeField] List<Furniture> furnitures;
    [SerializeField] GameObject prefabHolder;

    private void Awake()
    {
        LoadPrefabs();
    }

    void Start()
    {
        
    }

    void LoadPrefabs()
    {
        if (!prefabHolder) return;

        Debug.Log("Loading Prefabs");

        foreach (Transform child in prefabHolder.transform)
        {
            Furniture furniture = child.GetComponent<Furniture>();
            if (furniture != null)
            {
                if (!furnitures.Contains(furniture))
                {
                    furnitures.Add(furniture);
                }
            }
        }
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

    void Update()
    {
        
    }
}
