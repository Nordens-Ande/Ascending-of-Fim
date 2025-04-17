using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

//[System.Serializable]
//public class FurnitureData
//{
//    public RoomType roomType;
//    public Vector2Int size;
//    public Vector2Int frontDirection;
//    public List<Vector2Int> wallDirections = new List<Vector2Int>();

//    public FurnitureData() { }
//    public FurnitureData(RoomType roomType, Vector2Int size, Vector2Int frontDirection, List<Vector2Int> wallDirections)
//    {
//        this.roomType = roomType;
//        this.size = size;
//        this.frontDirection = frontDirection;
//        this.wallDirections = wallDirections;
//    }
//    public FurnitureData CloneRotated()
//    {
//        FurnitureData newFurniture = new FurnitureData();
//        newFurniture.size = new Vector2Int(size.y, size.x);
//        newFurniture.frontDirection = new Vector2Int(frontDirection.y, frontDirection.x);
//        foreach (var dir in wallDirections)
//            newFurniture.wallDirections.Add(new Vector2Int(dir.y, dir.x));
//        return newFurniture;
//    }
//    public FurnitureData CloneFlipped()
//    {
//        FurnitureData newFurniture = new FurnitureData();
//        newFurniture.size = size * -1;
//        newFurniture.frontDirection = frontDirection * -1;
//        foreach (Vector2Int dir in wallDirections)
//            newFurniture.wallDirections.Add(dir * -1);
//        return newFurniture;
//    }
//}

public class Furniture : MonoBehaviour
{
    public RoomType roomType;
    public Vector2Int size;
    public Vector2Int frontDirection;
    public List<Vector2Int> wallDirections = new List<Vector2Int>();

    [SerializeField] private bool drawDebug = false;
    
    void Start()
    {
        
    }

    public BoundsInt GetBounds()
    {
        return new BoundsInt((int)transform.position.x, (int)transform.position.y, 0, size.x, size.y, 1);
    }

    public List<Vector2Int> GetOccupiedTiles()
    {
        //List<Vector2Int> tiles = new List<Vector2Int>();
        //for (int x = 0; x < size.x; x++)
        //{
        //    for (int y = 0; y < size.y; y++)
        //    {
        //        tiles.Add(new Vector2Int((int)transform.position.x + x, (int)transform.position.z + y));
        //    }
        //}
        //return tiles;
        return GetOccupiedTiles(new Vector2Int((int)transform.position.x, (int)transform.position.z));
    }
    public List<Vector2Int> GetOccupiedTiles(Vector2Int tempPos)
    {
        List<Vector2Int> tiles = new List<Vector2Int>();
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                tiles.Add(new Vector2Int(tempPos.x + x, tempPos.y + y));
            }
        }
        return tiles;
    }

    private void OnDrawGizmos()
    {
        if (!drawDebug)
            return;

        DrawDirectionFromEdge(frontDirection, Color.yellow);
        foreach (Vector2Int dir in wallDirections)
            DrawDirectionFromEdge(dir, Color.red);
    }

    void DrawDirectionFromEdge(Vector2 dir, Color color)
    {
        if (dir == Vector2.zero) return;

        Vector2 normDir = dir.normalized;

        Vector3 startPoint = transform.position + new Vector3(((float)size.x * (1 + normDir.x)) / 2f, 1, ((float)size.y * (1 + normDir.y)) / 2f);
        Vector3 endPoint = startPoint + new Vector3(normDir.x / 2f, 0, normDir.y / 2f);

        Gizmos.color = color;
        Gizmos.DrawLine(startPoint, endPoint);
        Gizmos.DrawSphere(endPoint, 0.05f);
    }
    public Ray BuildRay(Vector2 dir)
    {
        Vector2 normDir = dir.normalized;
        Vector3 origo = transform.position + new Vector3(((float)size.x * (1 + normDir.x)) / 2f, 1, ((float)size.y * (1 + normDir.y)) / 2f);
        return new Ray(origo, new Vector3(normDir.x, 0, normDir.y));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
