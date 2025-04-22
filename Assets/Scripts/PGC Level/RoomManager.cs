using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public Material wallMaterial;
    public Material floorMaterial;
    public int maxFurniture;

    public RoomSettings(MinMaxInt sizeRange, Material wallMaterial, Material floorMaterial, int maxFurniture)
    {
        this.sizeRange = sizeRange;
        this.wallMaterial = wallMaterial;
        this.floorMaterial = floorMaterial;
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

        GenerateFurnitureLayout();

        foreach (Room room in rooms)
        {
            Material wall = roomSettings[room.Type].wallMaterial ? roomSettings[room.Type].wallMaterial : wallMaterial;
            Material floor = roomSettings[room.Type].floorMaterial ? roomSettings[room.Type].floorMaterial : floorMaterial;
            GameObject roomMesh = MeshBuilder.CreateRoomMesh(room, wall, floor);

            roomObjects.Add(roomMesh);
            roomDictionary[room] = roomMesh;
            //StartCoroutine(DelayedFurnitureCreation(room, roomMesh));
            //TryCreateFurniture(room);
            MeshBuilder.DecorateRoomMesh(roomMesh.transform.root, room);
        }
    }

    //private IEnumerator DelayedFurnitureLayout()
    //{
    //    //yield return null; //en frame
    //    yield return new WaitForSeconds(0.1f);
    //    //TryCreateFurniture(room);
    //    //MeshBuilder.DecorateRoomMesh(roomMesh.transform.root, room);
    //    //StartCoroutine(GenerateFurnitureLayout());
    //}

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

    private void GenerateFurnitureLayout()
    {
        Queue<Furniture> furnitureQueue = new Queue<Furniture>();

        foreach (Room room in rooms)
        {
            //furnitureQueue.Enqueue();

            RoomType type = room.Type;
            int failCount = 0;
            int maxFails = 20;
            int amountToGenerate = Random.Range(1, roomSettings[type].maxFurniture + 1);

            while (failCount < maxFails && room.FurnitureList.Count <= roomSettings[type].maxFurniture)
            {
                if (room.FurnitureList.Count >= amountToGenerate)
                    break;

                List<Furniture> availableFurnitures = roomPrefabManager.GetFurniture(room.Type);
                if (availableFurnitures == null || availableFurnitures.Count == 0)
                {
                    failCount++;
                    continue;
                }
                    

                bool addedFurnitureThisCycle = false;

                List<Vector2Int> roomTiles = room.GetOccupiedTiles();
                List<Vector2Int> doorTiles = room.GetDoorTiles(doorClearance);
                //List<Vector2Int> furnitureTiles = new List<Vector2Int>();
                //foreach (Furniture furniture in room.FurnitureList.Keys)
                //    furnitureTiles.AddRange(furniture.GetOccupiedTiles(room.FurnitureList[furniture]));

                Vector2Int spawnPos = roomTiles[Random.Range(0, roomTiles.Count)];
                Furniture selectedFurniture = availableFurnitures[Random.Range(0, availableFurnitures.Count)];

                if (room.FurnitureList.Any(f => f.Item1 == selectedFurniture) && !selectedFurniture.repeating)
                {
                    failCount++;
                    continue;
                }

                List<Vector2Int> selectedFurnitureTiles = selectedFurniture.GetOccupiedTiles(spawnPos);

                if (IsFurnitureSpaceFree(room, selectedFurniture, spawnPos))
                {
                    selectedFurniture.transform.position = new Vector3(spawnPos.x, 0, spawnPos.y);

                    float rayLength = 0.5f;

                    bool isClear = true;
                    foreach (Vector2Int dir in selectedFurniture.frontDirections)
                    {
                        Debug.Log("Amount furniture tiles: " + occupiedFurniturePositions.Count);

                        bool isRoom = CheckCollidingTilesAtDir(dir, selectedFurnitureTiles, true, roomTiles);
                        bool isFurniture = CheckCollidingTilesAtDir(dir, selectedFurnitureTiles, false, occupiedFurniturePositions.ToList());

                        Debug.Log($"Is dir {dir} a room tile {isRoom} or a furniture tile {isFurniture}");

                        if (!isRoom || isFurniture)
                            isClear = false;
                    }

                    bool isNextToWall = true;
                    foreach (Vector2Int dir in selectedFurniture.wallDirections)
                    {
                        if (CheckCollidingTilesAtDir(dir, selectedFurnitureTiles, true, roomTiles))
                            isNextToWall = false;
                    }

                    if (isClear && isNextToWall)
                    {
                        Debug.Log("Placed furniture: " + spawnPos);
                        addedFurnitureThisCycle = true;
                        AddFurniture(room, selectedFurniture, spawnPos);
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

    //REFERNS
    //void TryCreateFurniture(Room room)
    //{
    //    List<Furniture> availableFurnitures = roomPrefabManager.GetFurniture(room.Type);
    //    if (availableFurnitures == null || availableFurnitures.Count == 0)
    //        return;

    //    List<Vector2Int> roomTiles = room.GetOccupiedTiles();
    //    List<Vector2Int> doorTiles = room.GetDoorTiles(doorClearance);
    //    List<Vector2Int> furnitureTiles = new List<Vector2Int>();
    //    foreach ((Furniture f, Vector2Int v) furniture in room.FurnitureList)
    //        furnitureTiles.AddRange(furniture.f.GetOccupiedTiles(furniture.v));

    //    Vector2Int spawnPos = roomTiles[Random.Range(0, roomTiles.Count)];
    //    Furniture selectedFurniture = availableFurnitures[Random.Range(0, availableFurnitures.Count)];

    //    List<Vector2Int> selectedFurnitureTiles = selectedFurniture.GetOccupiedTiles(spawnPos);

    //    if (IsFurnitureSpaceFree(room, selectedFurniture, spawnPos))
    //    {
    //        selectedFurniture.transform.position = new Vector3(spawnPos.x, 0, spawnPos.y);

    //        float rayLength = 0.5f;

    //        bool isClear = CheckCollidingTilesAtDir(selectedFurniture.frontDirection, selectedFurnitureTiles, roomTiles, doorTiles)
    //                       && !CheckCollidingTilesAtDir(selectedFurniture.frontDirection, selectedFurnitureTiles, furnitureTiles);
    //        bool isNextToWall = true;
    //        foreach (Vector2Int dir in selectedFurniture.wallDirections)
    //        {
    //            if (CheckCollidingTilesAtDir(dir, selectedFurnitureTiles, roomTiles))
    //                isNextToWall = false;
    //        }

    //        if (isClear && isNextToWall)
    //        {
    //            Debug.Log("Placed furniture: " + spawnPos);
    //            AddFurniture(room, selectedFurniture, spawnPos);
    //        }
    //        else
    //        {
    //            Debug.Log("Failed placement");
    //            //AddFurniture(room, selectedFurniture, spawnPos);
    //        }
    //    }
    //}

    bool CheckCollidingTilesAtDir(Vector2Int dir, List<Vector2Int> compareTiles, bool allMustCollide, params List<Vector2Int>[] tileLists)
    {
        bool allCollided = true;

        foreach (List<Vector2Int> list in tileLists)
        {
            //if (list.Count == 0) continue;

            foreach (Vector2Int tile in compareTiles)
            {
                Vector2Int tileToCheck = tile + dir;
                if (debugger)
                    debugger.checkedTiles.Add(tileToCheck);
                if (list.Contains(tileToCheck))
                {
                    if (!allMustCollide) return true;
                }
                else
                {
                    if (allMustCollide) allCollided = false;
                }  
            }
        }

        if (allMustCollide) return allCollided;
        else return false;
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
        //tiles för det specifika rummet
        List<Vector2Int> roomTiles = room.GetOccupiedTiles();
        List<Vector2Int> doorTiles = room.GetDoorTiles(doorClearance);
        List<Vector2Int> furnitureTiles = new List<Vector2Int>();
        foreach ((Furniture f, Vector2Int v) furn in room.FurnitureList)
            furnitureTiles.AddRange(furn.f.GetOccupiedTiles(furn.v));

        foreach (Vector2Int tile in furniture.GetOccupiedTiles(position))
        {
            if (doorTiles.Contains(tile) || !roomTiles.Contains(tile) || furnitureTiles.Contains(tile))
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
        furniture.transform.position = new Vector3(pos.x, 0, pos.y);

        room.FurnitureList.Add((furniture, pos));
        Debug.Log($"{furniture.gameObject.transform.position} with vector3: {new Vector3(pos.x, 0, pos.y)}");

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
