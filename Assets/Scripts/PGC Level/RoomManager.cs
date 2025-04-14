using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class RoomManager : MonoBehaviour
{
    [SerializeField] Material m_Material;

    [SerializeField] int maxRooms = 10;
    [SerializeField] int minRoomSize = 2;
    [SerializeField] int maxRoomSize = 5;

    int doorClearance;

    private List<Room> rooms;
    private HashSet<Vector2Int> occupiedPositions = new HashSet<Vector2Int>();
    private HashSet<Vector2Int> occupiedDoorPositions = new HashSet<Vector2Int>();

    private Dictionary<RoomType, List<RoomType>> roomConnectionRules = new Dictionary<RoomType, List<RoomType>>()
    {
        { RoomType.Hallway, new List<RoomType> { RoomType.Corridor, RoomType.LivingRoom, RoomType.Bedroom, RoomType.Bathroom } },
        { RoomType.Corridor, new List<RoomType> { RoomType.Bedroom, RoomType.Closet, RoomType.Bathroom, RoomType.LivingRoom, RoomType.DiningRoom } },
        { RoomType.LivingRoom, new List<RoomType> { RoomType.DiningRoom, RoomType.Corridor } },
        { RoomType.Bedroom, new List<RoomType> { RoomType.Closet, RoomType.Bathroom } },
        { RoomType.DiningRoom, new List<RoomType> { RoomType.Corridor, RoomType.Bedroom, RoomType.LivingRoom } },
        { RoomType.Closet, new List<RoomType>() },
        { RoomType.Bathroom, new List<RoomType>() }
    };

    void Start()
    {
        rooms = new List<Room>();
        //rooms.Add(new Room(6, 2, 1, 0.1f, 1.25f, new Vector2Int(0, 0)));
        //rooms.Add(new Room(6, 2, 1, 0.1f, 1.25f, new Vector2Int(0, 2)));
        //rooms.Add(new Room(6, 2, 1, 0.1f, 1.25f, new Vector2Int(2, 4)));
        //rooms.Add(new Room(4, 2, 1, 0.1f, 1.25f, new Vector2Int(6, 2)));

        GenerateLayout();

        CheckNearbyRooms();

        foreach (Room room in rooms)
            MeshBuilder.CreateRoomMesh(room, m_Material, m_Material);

        //GameObject roomObject1 = MeshBuilder.CreateRoomMesh(rooms[0], m_Material, m_Material);
        //GameObject roomObject2 = MeshBuilder.CreateRoomMesh(rooms[1], m_Material, m_Material);
        //GameObject roomObject3 = MeshBuilder.CreateRoomMesh(rooms[2], m_Material, m_Material);
    }

    private void GenerateLayout()
    {
        Room initialRoom = new Room(4, 4, 1, 0.1f, 1.25f, Vector2Int.zero, RoomType.Hallway);
        AddRoom(initialRoom);

        Queue<Room> roomQueue = new Queue<Room>();
        roomQueue.Enqueue(initialRoom);

        while (rooms.Count < maxRooms && roomQueue.Count > 0)
        {
            Room currentRoom = roomQueue.Dequeue();
            List<RoomType> possibleConnections = roomConnectionRules[currentRoom.Type];

            foreach (RoomType type in possibleConnections)
            {
                if (rooms.Count >= maxRooms)
                    break;

                int width = Random.Range(minRoomSize, maxRoomSize + 1);
                int height = Random.Range(minRoomSize, maxRoomSize + 1);

                Vector2Int direction = GetRandomDirection();
                Vector2Int offset = GetOffset(direction, width, height);
                Vector2Int newPosition = currentRoom.Position + offset;

                Room newRoom = new Room(width, height, 1, 0.1f, 1.25f, newPosition, type);

                if (IsSpaceFree(newRoom) && IsRoomConnected(newRoom, direction))
                {
                    AddRoom(newRoom);
                    roomQueue.Enqueue(newRoom);
                }
            }
        }
    }

    private void CheckNearbyRooms()
    {
        if (rooms == null) return;

        for (int i = 0; i < rooms.Count; i++)
        {
            for (int j = 0; j < rooms.Count; j++)
            {
                TryCreateDoorsBetween(rooms[i], rooms[j]);
            }
        }
    }

    void TryCreateDoorsBetween(Room roomA, Room roomB)
    {
        BoundsInt boundsA = roomA.GetBounds();
        BoundsInt boundsB = roomB.GetBounds();

        float doorSize = roomA.DoorSize;

        // FRONT/DOWN & BACK/UP (A är över B)
        if (boundsA.yMin == boundsB.yMax)
        {
            int overlapMinX = Mathf.Max(boundsA.xMin, boundsB.xMin);
            int overlapMaxX = Mathf.Min(boundsA.xMax, boundsB.xMax);
            float overlap = overlapMaxX - overlapMinX;

            if (overlap >= doorSize)
            {
                int doorX = Mathf.RoundToInt(Random.Range(overlapMinX + doorSize / 2f, overlapMaxX - doorSize / 2f));
                roomA.Doorways.Add(new Vector2(doorX - boundsA.xMin, 0)); // Front vägg (A)
                roomB.Doorways.Add(new Vector2(doorX - boundsB.xMin, roomB.Height)); // Back vägg (B)
            }
        }

        // LEFT & RIGHT (A är höger om B)
        else if (boundsA.xMin == boundsB.xMax)
        {
            int overlapMinY = Mathf.Max(boundsA.yMin, boundsB.yMin);
            int overlapMaxY = Mathf.Min(boundsA.yMax, boundsB.yMax);
            float overlap = overlapMaxY - overlapMinY;

            if (overlap >= doorSize)
            {
                int doorZ = Mathf.RoundToInt(Random.Range(overlapMinY + doorSize / 2f, overlapMaxY - doorSize / 2f));
                roomA.Doorways.Add(new Vector2(0, doorZ - boundsA.yMin)); // Left (A)
                roomB.Doorways.Add(new Vector2(roomB.Width, doorZ - boundsB.yMin)); // Right vägg (B)
            }
        }
    }


    bool IsSpaceFree(Room room)
    {
        foreach (Vector2Int tile in room.GetOccupiedTiles())
        {
            if (occupiedPositions.Contains(tile))
                return false;
        }
        return true;
    }

    //Kontrollerar att rummet inte är för långt bort och att en dörr kan kopplas mellan dem. Lite dålig gjord då vi kollar alla tiles i det nya rummet, men det borde inte behövas att optimeras.
    bool IsRoomConnected(Room room, Vector2Int dir)
    {
        int minConnectedTiles = Mathf.CeilToInt(room.DoorSize);
        int connectedTiles = 0;

        foreach (Vector2Int tile in room.GetOccupiedTiles())
        {
            if (occupiedPositions.Contains(tile + (dir * -1)))
                connectedTiles++;
        }
        return connectedTiles >= minConnectedTiles;
    }

    void AddRoom(Room room)
    {
        rooms.Add(room);
        foreach (Vector2Int tile in room.GetOccupiedTiles())
        {
            occupiedPositions.Add(tile);
        }
    }

    //void AddDoorSpace(Room room)
    //{
    //    int maxX 
    //}

    private Vector2Int GetRandomDirection()
    {
        Vector2Int[] directions = new Vector2Int[]
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        return directions[Random.Range(0, directions.Length)];
    }

    Vector2Int GetOffset(Vector2Int direction, int width, int height)
    {
        // Offset so the new room is placed adjacent to the parent room
        if (direction == Vector2Int.up)
            return new Vector2Int(0, height);
        if (direction == Vector2Int.down)
            return new Vector2Int(0, -height);
        if (direction == Vector2Int.left)
            return new Vector2Int(-width, 0);
        if (direction == Vector2Int.right)
            return new Vector2Int(width, 0);

        return Vector2Int.zero;
    }


    void Update()
    {
        
    }
}
