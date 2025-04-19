using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;
using static UnityEditor.PlayerSettings;

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
[System.Serializable]
public struct RoomSettings
{
    public MinMaxInt sizeRange;
    public int maxFurniture;

    public RoomSettings(MinMaxInt sizeRange, int maxFurniture)
    {
        this.sizeRange = sizeRange;
        this.maxFurniture = maxFurniture;
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
    //[SerializeField] MinMaxInt hallwaySizeRange;
    //[SerializeField] MinMaxInt corridorSizeRange;
    //[SerializeField] MinMaxInt livingRoomSizeRange;
    //[SerializeField] MinMaxInt bedroomSizeRange;
    //[SerializeField] MinMaxInt diningRoomSizeRange;
    //[SerializeField] MinMaxInt closetSizeRange;
    //[SerializeField] MinMaxInt bathroomSizeRange;
    //[SerializeField] RoomSettings test;

    [SerializeField] RoomSettings hallwaySettings;
    [SerializeField] RoomSettings corridorSettings;
    [SerializeField] RoomSettings livingRoomSettings;
    [SerializeField] RoomSettings bedroomSettings;
    [SerializeField] RoomSettings diningRoomSettings;
    [SerializeField] RoomSettings closetSettings;
    [SerializeField] RoomSettings bathroomSettings;

    TileDebugger debugger;
    RoomPrefabManager roomPrefabManager;

    private List<Room> rooms;
    private List<GameObject> roomObjects;
    private Dictionary<Room, GameObject> roomDictionary; //koppla refernserna mellan rummen här och gör en metod, lik GenerateRoomLayout fastän för möbler och använd detta bibliotek för referenser, gör en coroutine på hela möbel metoden sen

    private List<GameObject> furnitureObjects;

    private HashSet<Vector2Int> occupiedRoomPositions;
    private HashSet<Vector2Int> occupiedDoorPositions;
    private HashSet<Vector2Int> occupiedFurniturePositions;

    private Dictionary<RoomType, MinMaxInt> roomSizes;
    private Dictionary<RoomType, RoomSettings> roomSettings;

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
        //roomSizes = new Dictionary<RoomType, MinMaxInt>()
        //{
        //    { RoomType.Hallway, hallwaySizeRange},
        //    { RoomType.Corridor, corridorSizeRange},
        //    { RoomType.LivingRoom, livingRoomSizeRange},
        //    { RoomType.Bedroom, bedroomSizeRange},
        //    { RoomType.DiningRoom, diningRoomSizeRange},
        //    { RoomType.Closet, closetSizeRange},
        //    { RoomType.Bathroom, bathroomSizeRange}
        //};
        roomSettings = new Dictionary<RoomType, RoomSettings>()
        {
            { RoomType.Hallway, hallwaySettings},
            { RoomType.Corridor, corridorSettings},
            { RoomType.LivingRoom, livingRoomSettings},
            { RoomType.Bedroom, bedroomSettings},
            { RoomType.DiningRoom, diningRoomSettings},
            { RoomType.Closet, closetSettings},
            { RoomType.Bathroom, bathroomSettings}
        };

        debugger = GetComponent<TileDebugger>();
        roomPrefabManager = GetComponent<RoomPrefabManager>();
    }

    void Start()
    {
        reroll = false;

        rooms = new List<Room>();
        roomObjects = new List<GameObject>();
        roomDictionary = new Dictionary<Room, GameObject>();

        occupiedRoomPositions = new HashSet<Vector2Int>();
        occupiedDoorPositions = new HashSet<Vector2Int>();
        occupiedFurniturePositions = new HashSet<Vector2Int>();

        GenerateRoomLayout();

        CheckNearbyRooms();

        foreach (Room room in rooms)
        {
            GameObject roomMesh = MeshBuilder.CreateRoomMesh(room, wallMaterial, floorMaterial);
            roomObjects.Add(roomMesh);
            roomDictionary[room] = roomMesh;
            //StartCoroutine(DelayedFurnitureCreation(room, roomMesh));
            //TryCreateFurniture(room);
            //MeshBuilder.DecorateRoomMesh(roomMesh.transform.root, room);
        }

        StartCoroutine(DelayedFurnitureLayout());

        //foreach (Room room in rooms)
        //{
        //    MeshBuilder.DecorateRoomMesh(roomDictionary[room].transform.root, room);
        //}
    }

    private IEnumerator DelayedFurnitureLayout()
    {
        //yield return null; //en frame
        yield return new WaitForSeconds(0.1f);
        //TryCreateFurniture(room);
        //MeshBuilder.DecorateRoomMesh(roomMesh.transform.root, room);
        StartCoroutine(GenerateFurnitureLayout());
    }

    private void GenerateRoomLayout()
    {
        Room initialRoom = new Room(initWidth, initHeight, wallHeight, wallThickness, doorSize, Vector2Int.zero, RoomType.Hallway);
        AddRoom(initialRoom);

        Queue<Room> roomQueue = new Queue<Room>();
        roomQueue.Enqueue(initialRoom);

        int amountToGenerate = Random.Range(roomAmountRange.min, roomAmountRange.max + 1);
        Debug.Log("Max amount to generate: " + amountToGenerate);

        int failCount = 0;
        int maxFails = 1000; //övregräns för att hindra oändliga loops, när detta uppnås så stoppas generationen.

        while ((rooms.Count < roomAmountRange.max || rooms.Count < roomAmountRange.min) && failCount < maxFails)
        {
            if (roomQueue.Count == 0)
            {
                //H?r väljer vi (random) ett nytt room att generera från, görs ifall algoritmen "fastnar"
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

                MinMaxInt sizeRange = roomSettings[type].sizeRange;

                int width = Random.Range(sizeRange.min, sizeRange.max + 1);
                int height = Random.Range(sizeRange.min, sizeRange.max + 1);

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

    private IEnumerator GenerateFurnitureLayout()
    {
        //Queue<Furniture> furnitureQueue = new Queue<Furniture>();

        foreach (Room room in rooms)
        {
            yield return null;
            yield return null; //väntar 2 frames

            RoomType type = room.Type;
            int failCount = 0;
            int maxFails = 50;

            while (failCount < maxFails && room.FurnitureList.Count <= roomSettings[type].maxFurniture)
            {
                List<Furniture> availableFurnitures = roomPrefabManager.GetFurniture(room.Type);
                if (availableFurnitures == null || availableFurnitures.Count == 0)
                {
                    failCount++;
                    continue;
                }

                bool addedFurnitureThisCycle = false;

                List<Vector2Int> currRoomTiles = room.GetOccupiedTiles();
                Vector2Int spawnPos = currRoomTiles[Random.Range(0, currRoomTiles.Count)];
                Furniture selectedFurniture = availableFurnitures[Random.Range(0, availableFurnitures.Count)];

                //if (room.FurnitureList.Contains(selectedFurniture))
                //{
                //    failCount++;
                //    continue;
                //}

                if (IsFurnitureSpaceFree(room, selectedFurniture, spawnPos))
                {
                    //Debug.Log("Trying ray for furniture");
                    selectedFurniture.transform.position = new Vector3(spawnPos.x, 0, spawnPos.y);

                    float rayLength = 0.5f;

                    bool isClear = IsClearAhead(selectedFurniture.BuildRay(selectedFurniture.frontDirection), rayLength, null, selectedFurniture.drawDebug);
                    bool IsNextToWall = true;
                    foreach (Vector2Int dir in selectedFurniture.wallDirections)
                    {
                        if (IsClearAhead(selectedFurniture.BuildRay(dir), rayLength, LayerMask.GetMask("Wall"), selectedFurniture.drawDebug))
                            IsNextToWall = false;
                    }

                    if (isClear && IsNextToWall)
                    {
                        Debug.Log("Placed furniture: " + spawnPos);
                        AddFurniture(room, selectedFurniture, spawnPos);
                        MeshBuilder.CreateFurniture(roomDictionary[room].transform.root, selectedFurniture.gameObject);
                        addedFurnitureThisCycle = true;
                    }
                    else
                    {
                        Debug.Log("Failed placement");
                        //AddFurniture(room, selectedFurniture, spawnPos);
                    }
                }

                if (!addedFurnitureThisCycle)
                    failCount++;
                else
                    failCount = 0; // Reset fails if successful
            }

            //MeshBuilder.DecorateRoomMesh(roomDictionary[room].transform.root, room);
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

        // FRONT/DOWN & BACK/UP (A ?r ?ver B)
        if (boundsA.yMin == boundsB.yMax)
        {
            int overlapMinX = Mathf.Max(boundsA.xMin, boundsB.xMin);
            int overlapMaxX = Mathf.Min(boundsA.xMax, boundsB.xMax);
            float overlap = overlapMaxX - overlapMinX;

            if (overlap >= doorSize)
            {
                int doorX = Mathf.RoundToInt(Random.Range(overlapMinX + doorSize / 2f, overlapMaxX - doorSize / 2f));
                roomA.Doorways.Add(new Vector2(doorX - boundsA.xMin, 0)); // Front v?gg (A)
                roomB.Doorways.Add(new Vector2(doorX - boundsB.xMin, roomB.Height)); // Back v?gg (B)
            }
        }

        // LEFT & RIGHT (A ?r h?ger om B)
        else if (boundsA.xMin == boundsB.xMax)
        {
            int overlapMinY = Mathf.Max(boundsA.yMin, boundsB.yMin);
            int overlapMaxY = Mathf.Min(boundsA.yMax, boundsB.yMax);
            float overlap = overlapMaxY - overlapMinY;

            if (overlap >= doorSize)
            {
                int doorZ = Mathf.RoundToInt(Random.Range(overlapMinY + doorSize / 2f, overlapMaxY - doorSize / 2f));
                roomA.Doorways.Add(new Vector2(0, doorZ - boundsA.yMin)); // Left (A)
                roomB.Doorways.Add(new Vector2(roomB.Width, doorZ - boundsB.yMin)); // Right v?gg (B)
            }
        }

        AddDoorSpace(roomA);
    }

    void TryCreateFurniture(Room room)
    {
        List<Furniture> availableFurnitures = roomPrefabManager.GetFurniture(room.Type);
        if (availableFurnitures == null || availableFurnitures.Count == 0)
            return;

        BoundsInt bounds = room.GetBounds();
        List<Vector2Int> currRoomTiles = room.GetOccupiedTiles();
        Vector2Int spawnPos = currRoomTiles[Random.Range(0, currRoomTiles.Count)];
        Furniture selectedFurniture = availableFurnitures[Random.Range(0, availableFurnitures.Count)];

        if (IsFurnitureSpaceFree(room, selectedFurniture, spawnPos))
        {
            Debug.Log("Trying ray for furniture");
            selectedFurniture.transform.position = new Vector3(spawnPos.x, 0, spawnPos.y);

            //RaycastHit hit;
            //Ray ray = selectedFurniture.BuildRay(selectedFurniture.frontDirection);
            ////Ray ray = new Ray(new Vector3(spawnPos.x, 1, spawnPos.y), new Vector3(selectedFurniture.frontDirection.x, 0, selectedFurniture.frontDirection.y));

            ////Ritar ut Debug rays i 10 sekunder (gr?n = lyckad - r?d = misslyckad)
            //Color rayColor = Physics.Raycast(ray, out hit, 0.5f) ? Color.red : Color.green;
            //Debug.DrawRay(ray.origin, ray.direction * 0.5f, rayColor, 10f);

            //// Now handle the placement logic
            //if (Physics.Raycast(ray, out hit, 0.5f))
            //{
            //    Debug.Log("Failed with placement, hit: " + hit.transform.name);
            //    //AddFurniture(room, selectedFurniture, spawnPos);
            //}
            //else
            //{
            //    Debug.Log("Placed furniture");
            //    AddFurniture(room, selectedFurniture, spawnPos);

            //    //room.FurnitureList.Add(furnitures[0]);
            //}

            float rayLength = 0.5f;

            bool isClear = IsClearAhead(selectedFurniture.BuildRay(selectedFurniture.frontDirection), rayLength, null, selectedFurniture.drawDebug);
            bool IsNextToWall = true;
            foreach (Vector2Int dir in selectedFurniture.wallDirections)
            {
                if (IsClearAhead(selectedFurniture.BuildRay(dir), rayLength, LayerMask.GetMask("Wall"), selectedFurniture.drawDebug))
                    IsNextToWall = false;
            }

            if (isClear && IsNextToWall)
            {
                Debug.Log("Placed furniture: " + spawnPos);
                AddFurniture(room, selectedFurniture, spawnPos);
            }
            else
            {
                Debug.Log("Failed placement");
                //AddFurniture(room, selectedFurniture, spawnPos);
            }
        }
    }

    bool IsClearAhead(Ray ray, float length, LayerMask? layer = null, bool debug = false)
    {
        RaycastHit hit;
        Color rayColor = Physics.Raycast(ray, out hit, length) ? Color.red : Color.green;

        if (layer.HasValue && layer.Value != 0)
        {
            rayColor = Physics.Raycast(ray, out hit, length, layer.Value) ? Color.red : Color.green;

            if (Physics.Raycast(ray, out hit, length, layer.Value))
            {
                if (debug)
                    Debug.Log($"Hit obstacle: {hit.transform.name} with layer: {layer.Value}");
                return false;
            }
        }
        else
        {
            if (Physics.Raycast(ray, out hit, length))
            {
                if (debug)
                    Debug.Log("Hit obstacle: " + hit.transform.name);
                return false;
            }
        }

        if (debug)
            Debug.DrawRay(ray.origin, ray.direction * length, rayColor, 10f);

        return true;
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

    //Kontrollerar att rummet inte ?r f?r l?ngt bort och att en d?rr kan kopplas mellan dem. Lite d?lig gjord d? vi kollar alla tiles i det nya rummet, men det borde inte beh?vas att optimeras.
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
        //Vector2Int offset = pos - room.Position;
        //furniture.transform.position = new Vector3(pos.x, 0, pos.y);

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
