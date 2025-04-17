using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

[System.Serializable]
public struct MinMaxInt
{
    public int min;
    public int max;

    public MinMaxInt(int min, int max)
    {
        this.min = min;
        this.max = max;
    }
}

public class RoomManager : MonoBehaviour
{
    [SerializeField] bool reroll = false;
    //[SerializeField] bool debug = false;
    [Space]

    [Header("Room Settings")]
    [SerializeField] Material wallMaterial;
    [SerializeField] Material floorMaterial;
    [SerializeField] int wallHeight;
    [SerializeField] float wallThickness;
    [SerializeField] float doorSize;

    [Header("Initial Room")]
    [SerializeField] int initWidth;
    [SerializeField] int initHeight;
    //[SerializeField] Vector2Int initPosition;

    [Header("Apartment Settings")]
    [SerializeField] MinMaxInt roomAmountRange;
    [SerializeField] int doorClearance;

    [Header("Specific Room Sizes")]
    [SerializeField] MinMaxInt hallwaySizeRange;
    [SerializeField] MinMaxInt corridorSizeRange;
    [SerializeField] MinMaxInt livingRoomSizeRange;
    [SerializeField] MinMaxInt bedroomSizeRange;
    [SerializeField] MinMaxInt diningRoomSizeRange;
    [SerializeField] MinMaxInt closetSizeRange;
    [SerializeField] MinMaxInt bathroomSizeRange;


    TileDebugger debugger;
    RoomPrefabManager roomPrefabManager;

    private List<Room> rooms;
    private List<GameObject> roomObjects;
    private List<GameObject> furnitureObjects;
    private HashSet<Vector2Int> occupiedRoomPositions;
    private HashSet<Vector2Int> occupiedDoorPositions;
    private HashSet<Vector2Int> occupiedFurniturePositions;

    private Dictionary<RoomType, MinMaxInt> roomSizes;

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

    private void Awake()
    {
        roomSizes = new Dictionary<RoomType, MinMaxInt>()
        {
            { RoomType.Hallway, hallwaySizeRange},
            { RoomType.Corridor, corridorSizeRange},
            { RoomType.LivingRoom, livingRoomSizeRange},
            { RoomType.Bedroom, bedroomSizeRange},
            { RoomType.DiningRoom, diningRoomSizeRange},
            { RoomType.Closet, closetSizeRange},
            { RoomType.Bathroom, bathroomSizeRange}
        };

        debugger = GetComponent<TileDebugger>();
        roomPrefabManager = GetComponent<RoomPrefabManager>();
    }

    void Start()
    {
        rooms = new List<Room>();
        roomObjects = new List<GameObject>();

        occupiedRoomPositions = new HashSet<Vector2Int>();
        occupiedDoorPositions = new HashSet<Vector2Int>();
        occupiedFurniturePositions = new HashSet<Vector2Int>();

        GenerateLayout();

        CheckNearbyRooms();

        foreach (Room room in rooms)
        {
            GameObject roomMesh = MeshBuilder.CreateRoomMesh(room, wallMaterial, floorMaterial);
            roomObjects.Add(roomMesh);
            TryCreateFurniture(room);
            MeshBuilder.DecorateRoomMesh(roomMesh.transform.root, room);
        }

    }

    private void GenerateLayout()
    {
        Room initialRoom = new Room(initWidth, initHeight, wallHeight, wallThickness, doorSize, Vector2Int.zero, RoomType.Hallway);
        AddRoom(initialRoom);

        Queue<Room> roomQueue = new Queue<Room>();
        roomQueue.Enqueue(initialRoom);

        int amountToGenerate = Random.Range(roomAmountRange.min, roomAmountRange.max + 1);
        Debug.Log("Max amount to generate: " + amountToGenerate);

        int failCount = 0;
        int maxFails = 1000; //Övregräns för att hindra oändliga loops, när detta uppnås så stoppas generationen.

        while ((rooms.Count < roomAmountRange.max || rooms.Count < roomAmountRange.min) && failCount < maxFails)
        {
            if (roomQueue.Count == 0)
            {
                //Här väljer vi (random) ett nytt room att generera från, görs ifall algoritmen "fastnar"
                Room fallbackRoom = rooms[Random.Range(0, rooms.Count)];
                roomQueue.Enqueue(fallbackRoom);
            }

            Room currentRoom = roomQueue.Dequeue();
            List<RoomType> possibleConnections = roomConnectionRules[currentRoom.Type];

            bool addedRoomThisCycle = false;

            foreach (RoomType type in possibleConnections)
            {
                if (rooms.Count >= amountToGenerate)
                    break;

                //int width = Random.Range(minRoomSize, maxRoomSize + 1);
                //int height = Random.Range(minRoomSize, maxRoomSize + 1);

                int width = Random.Range(roomSizes[type].min, roomSizes[type].max + 1);
                int height = Random.Range(roomSizes[type].min, roomSizes[type].max + 1);

                Vector2Int direction = GetRandomDirection();
                Vector2Int offset = GetOffset(direction, width, height);
                Vector2Int newPosition = currentRoom.Position + offset;

                Room newRoom = new Room(width, height, wallHeight, wallThickness, doorSize, newPosition, type);

                if (IsRoomSpaceFree(newRoom) && IsRoomConnected(newRoom, direction))
                {
                    AddRoom(newRoom);
                    roomQueue.Enqueue(newRoom);
                    addedRoomThisCycle = true;
                }
            }

            if (!addedRoomThisCycle)
                failCount++;
            else
                failCount = 0; // Reset fails if successful
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

        AddDoorSpace(roomA);
    }

    void TryCreateFurniture(Room room)
    {
        List<Furniture> furnitures = roomPrefabManager.GetFurniture(room.Type);
        if (furnitures == null || furnitures.Count == 0)
            return;

        BoundsInt bounds = room.GetBounds();
        List<Vector2Int> currRoomTiles = room.GetOccupiedTiles();
        Vector2Int spawnPos = currRoomTiles[Random.Range(0, currRoomTiles.Count)];
        Furniture selectedFurniture = furnitures[Random.Range(0, furnitures.Count)];

        if (IsFurnitureSpaceFree(room, selectedFurniture, spawnPos))
        {
            Debug.Log("Trying ray for furniture");

            RaycastHit hit;
            //Ray ray = selectedFurniture.BuildRay(selectedFurniture.frontDirection);
            Ray ray = new Ray(new Vector3(spawnPos.x, 1, spawnPos.y), new Vector3(selectedFurniture.frontDirection.x, 0, selectedFurniture.frontDirection.y));

            // Draw the ray in the Scene view for 1 second (red if it hits, green if not)
            Color rayColor = Physics.Raycast(ray, out hit, 0.5f) ? Color.red : Color.green;
            Debug.DrawRay(ray.origin, ray.direction * 0.5f, rayColor, 15f);

            // Now handle the placement logic
            if (Physics.Raycast(ray, out hit, 0.5f))
            {
                Debug.Log("Failed with placement, hit: " + hit.transform.name);
            }
            else
            {
                Debug.Log("Placed furniture");
                AddFurniture(room, selectedFurniture, spawnPos);

                //room.FurnitureList.Add(furnitures[0]);
            }
        }
    }


    bool IsRoomSpaceFree(Room room)
    {
        foreach (Vector2Int tile in room.GetOccupiedTiles())
        {
            if (occupiedRoomPositions.Contains(tile))
                return false;
        }
        return true;
    }
    bool IsFurnitureSpaceFree(Room room, Furniture furniture, Vector2Int position)
    {
        List<Vector2Int> roomTiles = room.GetOccupiedTiles();
        List<Vector2Int> doorTiles = room.GetDoorTiles(doorClearance);

        foreach (Vector2Int tile in furniture.GetOccupiedTiles(position))
        {
            if (doorTiles.Contains(tile) || !roomTiles.Contains(tile))
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
            if (occupiedRoomPositions.Contains(tile + (dir * -1)))
                connectedTiles++;
        }
        return connectedTiles >= minConnectedTiles;
    }

    void AddRoom(Room room)
    {
        rooms.Add(room);
        foreach (Vector2Int tile in room.GetOccupiedTiles())
        {
            occupiedRoomPositions.Add(tile);
            if (debugger)
                debugger.roomTiles.Add(tile);
        }
    }

    void AddFurniture(Room room, Furniture furniture, Vector2Int pos)
    {
        Vector2Int offset = pos - room.Position;
        furniture.transform.position = new Vector3(pos.x, 0, pos.y);

        room.FurnitureList.Add(furniture);

        foreach (Vector2Int tile in furniture.GetOccupiedTiles())
        {
            occupiedFurniturePositions.Add(tile);
            if (debugger)
                debugger.furnitureTiles.Add(tile);
        }
    }

    void AddDoorSpace(Room room)
    {
        foreach (Vector2Int tile in room.GetDoorTiles(doorClearance))
        {
            occupiedRoomPositions.Add(tile);
            if (debugger)
                debugger.doorTiles.Add(tile);
        }
    }

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
        if (reroll)
        {
            reroll = false;

            foreach(GameObject room in roomObjects)
                Destroy(room);
            
            if (debugger)
                debugger.ClearTiles();

            Awake();

            Start();
        }
    }
}
