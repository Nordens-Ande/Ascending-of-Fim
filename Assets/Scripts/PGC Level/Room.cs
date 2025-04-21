using System.Collections.Generic;
using UnityEngine;

public enum RoomType
{
    Hallway,
    Corridor,
    Bedroom,
    LivingRoom,
    DiningRoom,
    Bathroom,
    Closet
}

public class Room : ITileable
{
    public int Width, Height;
    public float WallHeight, WallThickness;
    public float DoorSize;
    public Vector2Int Position;
    public HashSet<Vector2> Doorways = new HashSet<Vector2>();
    public RoomType Type;

    public List<(Furniture furniture, Vector2Int position)> FurnitureList = new List<(Furniture furniture, Vector2Int position)>();
    //public GameObject RoomObject { get; set; }

    public Room(int width, int height, float wallHeight, float wallThickness, float doorSize, Vector2Int position, RoomType type)
    {
        Width = width;
        Height = height;
        WallHeight = wallHeight;
        WallThickness = wallThickness;
        DoorSize = doorSize;
        Position = position;
        Type = type;
    }

    public List<float> GetDoorways(Vector2 dir)
    {
        List<float> result = new List<float>();
        BoundsInt bounds = GetBounds();

        foreach (Vector2 door in Doorways)
        {
            float ?pos = dir switch
            {
                {x:0, y:1}  when door.y == bounds.yMax - bounds.yMin => door.x, //back/up
                {x:0, y:-1} when door.y == bounds.yMin - bounds.yMin => door.x, //front/down
                {x:1, y:0}  when door.x == bounds.xMax - bounds.xMin => door.y, //h?ger
                {x:-1, y:0} when door.x == bounds.xMin - bounds.xMin => door.y, //v?nster
                _ => null
            };

            if (pos != null)
                result.Add((float)pos);
        }

        result.Sort();
        return result;
    }

    public BoundsInt GetBounds()
    {
        return new BoundsInt(Position.x, Position.y, 0, Width, Height, 1);
    }

    public List<Vector2Int> GetOccupiedTiles()
    {
        List<Vector2Int> tiles = new List<Vector2Int>();
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                tiles.Add(new Vector2Int(Position.x + x, Position.y + y));
            }
        }
        return tiles;
    }

    public List<Vector2Int> GetDoorTiles(int doorClearance)
    {
        List<Vector2Int> tiles = new List<Vector2Int>();
        foreach (Vector2 door in Doorways)
        {
            Vector2Int doorInt = new Vector2Int((int)door.x, (int)door.y);

            //Back && front
            if (door.y == 0 || door.y == Height)
            {
                for (int x = -Mathf.CeilToInt(DoorSize / 2f); x < Mathf.CeilToInt(DoorSize / 2f); x++)
                {
                    for (int y = -doorClearance; y < doorClearance; y++)
                    {
                        tiles.Add(new Vector2Int(Position.x + (int)door.x + x, Position.y + (int)door.y + y));
                    }
                }
            }
            //Right && Left
            if (door.x == 0 || door.x == Width)
            {
                for (int x = -doorClearance; x < doorClearance; x++)
                {
                    for (int y = -Mathf.CeilToInt(DoorSize / 2f); y < Mathf.CeilToInt(DoorSize / 2f); y++)
                    {
                        tiles.Add(new Vector2Int(Position.x + (int)door.x + x, Position.y + (int)door.y + y));
                    }
                }
            }
        }
        return tiles;
    }

    public bool Equals(Room other)
    {
        if (this == other) return true;
        if (Doorways.Count != other.Doorways.Count) return false;
        foreach (Vector2 d in Doorways) 
            if (!other.Doorways.Contains(d))
                return false;
        bool changedRoom = 
            Width != other.Width ||
            Height != other.Height ||
            Position.x != other.Position.x ||
            Position.y != other.Position.y ||
            WallHeight != other.WallHeight ||
            WallThickness != other.WallThickness;
        return changedRoom;
    }

    public bool Equals(int width, int height, float wallHeight, float wallThickness, Vector2Int pos, HashSet<Vector2> doorways)
    {
        if (Doorways.Count != doorways.Count) return false;
        foreach (Vector2 d in Doorways)
            if (!doorways.Contains(d))
                return false;
        bool changedRoom =
            Width != width ||
            Height != height ||
            Position.x != pos.x ||
            Position.y != pos.y ||
            WallHeight != wallHeight ||
            WallThickness != wallThickness;
        return changedRoom;
    }
}
