using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public int Width, Height;
    public float WallHeight, WallThickness;
    public Vector2Int Position;
    public HashSet<Vector2> Doorways = new HashSet<Vector2>();
    //Nytt sätt för dörringångarna måste göras.
    //Försök implementera definitioner för dörringångarna åt varje riktning, detta underlättar för implementationen av dörren

    public GameObject RoomObject;

    public Room(int width, int height, float wallHeight, float wallThickness, Vector2Int position)
    {
        Width = width;
        Height = height;
        WallHeight = wallHeight;
        WallThickness = wallThickness;
        Position = position;
    }

    public List<float> GetDoorways(Vector2 dir)
    {
        List<float> result = new List<float>();
        BoundsInt bounds = GetBounds();

        foreach (Vector2 door in Doorways)
        {
            float ?pos = dir switch
            {
                {x:0, y:1}  when door.y == bounds.yMax => door.x, //fram
                {x:0, y:-1} when door.y == bounds.yMin => door.x, //bak
                {x:1, y:0}  when door.x == bounds.xMax => door.y, //höger
                {x:-1, y:0} when door.x == bounds.xMin => door.y, //vänster
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
