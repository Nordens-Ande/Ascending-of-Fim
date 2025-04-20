using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Furniture : MonoBehaviour, ITileable
{
    public RoomType roomType;
    public Vector2Int size;
    public Vector2Int frontDirection;
    public List<Vector2Int> wallDirections = new List<Vector2Int>();

    public bool drawDebug = false;
    
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

        Vector3 cubeSize = new Vector3(size.x, Mathf.Clamp(2, 1, (size.x + size.y) / 2f), size.y);
        Gizmos.DrawWireCube(transform.position + cubeSize / 2f, cubeSize);

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
        Vector3 offset = new Vector3(normDir.x, 0, normDir.y) * -0.1f; //Offset för att se till så att ray-en inte skapas inuti en väg (och aldrig kolliderar), den är halverad av nån anledning
        Vector3 origo = transform.position + new Vector3(((float)size.x * (1 + normDir.x)) / 2f, 1, ((float)size.y * (1 + normDir.y)) / 2f);
        return new Ray(origo + offset, new Vector3(normDir.x, 0, normDir.y));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
