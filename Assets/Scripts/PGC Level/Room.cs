using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public int Width, Height;
    public float WallHeight, WallThickness;
    public Vector2Int Position;
    public HashSet<Vector2Int> Doorways = new HashSet<Vector2Int>();

    public GameObject RoomObject;

    public Room(int width, int height, float wallHeight, float wallThickness, Vector2Int position)
    {
        Width = width;
        Height = height;
        WallHeight = wallHeight;
        WallThickness = wallThickness;
        Position = position;
    }

    public BoundsInt GetBounds()
    {
        return new BoundsInt(Position.x, Position.y, 0, Width, Height, 1);
    }

    public bool Equals(Room other)
    {
        if (this == other) return true;
        if (Doorways.Count != other.Doorways.Count) return false;
        foreach (Vector2Int d in Doorways) 
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

    public bool Equals(int width, int height, float wallHeight, float wallThickness, Vector2Int pos, HashSet<Vector2Int> doorways)
    {
        if (Doorways.Count != doorways.Count) return false;
        foreach (Vector2Int d in Doorways)
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
