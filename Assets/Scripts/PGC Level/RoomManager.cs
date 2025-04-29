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

    [SerializeField] GameObject player;
    //[SerializeField] bool debug = false;
    [Space]


    [Header("Room Settings")]
    [SerializeField] Material wallMaterial;
    [SerializeField] Material floorMaterial;
    [SerializeField] int wallHeight;
    [SerializeField] float wallThickness;
    [SerializeField] float doorSize;

    [Header("Initial Room")]
    [SerializeField] RoomType initType;
    [SerializeField] int initWidth;
    [SerializeField] int initHeight;
    //[SerializeField] Vector2Int initPosition;

    [Header("Apartment Settings")]
    [SerializeField] MinMaxInt roomAmountRange;
    //[SerializeField] int maxElevators;
    [SerializeField] int doorClearance;
    [SerializeField] int maxRoomFails = 1000;
    [SerializeField] int maxFurnitureFails = 100;

    [Header("Specific Room Sizes")]
    [SerializeField] RoomSettings hallwaySettings;
    [SerializeField] RoomSettings corridorSettings;
    [SerializeField] RoomSettings livingRoomSettings;
    [SerializeField] RoomSettings bedroomSettings;
    [SerializeField] RoomSettings diningRoomSettings;
    [SerializeField] RoomSettings closetSettings;
    [SerializeField] RoomSettings bathroomSettings;
    RoomSettings elevatorSettings;

    TileDebugger debugger;
    RoomPrefabManager roomPrefabManager;
    NavMeshBaker navMeshBaker;
    [SerializeField] EnemySpawnPointManager enemySpawnPointManager;

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
        { RoomType.Elevator, new List<RoomType>() { } },
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
            { RoomType.Elevator, elevatorSettings },
            { RoomType.Hallway, hallwaySettings},
            { RoomType.Corridor, corridorSettings},
            { RoomType.LivingRoom, livingRoomSettings},
            { RoomType.Bedroom, bedroomSettings},
            { RoomType.DiningRoom, diningRoomSettings},
            { RoomType.Closet, closetSettings},
            { RoomType.Bathroom, bathroomSettings}
        };
        elevatorSettings = new RoomSettings(new MinMaxInt(2, 2), wallMaterial, floorMaterial, 1);

        debugger = GetComponent<TileDebugger>();
        roomPrefabManager = GetComponent<RoomPrefabManager>();
        navMeshBaker = GetComponent<NavMeshBaker>();
    }

    void Start()
    {
        reroll = false;
        if (player)
            player.transform.position = new Vector3(initWidth / 2f, 3, initHeight / 2f);

        rooms = new List<Room>();
        roomObjects = new List<GameObject>();
        roomDictionary = new Dictionary<Room, GameObject>();

        occupiedRoomPositions = new HashSet<Vector2Int>();
        occupiedDoorPositions = new HashSet<Vector2Int>();
        occupiedFurniturePositions = new HashSet<Vector2Int>();

        GenerateRoomLayout();

        //RoofGeneration();
        foreach (List<Vector2Int> hole in FindHoles(occupiedRoomPositions))
        {
            foreach (Vector2Int tile in hole)
            {
                debugger.holeTiles.Add(tile);
            }
        }

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

        if(navMeshBaker != null)
        {
            StartCoroutine(navMeshBaker.BakeNavMesh());
        }

        if(enemySpawnPointManager != null)
        {
            enemySpawnPointManager.GetSpawnPoints();
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
        Room initialRoom = new Room(initWidth, initHeight, wallHeight, wallThickness, doorSize, Vector2Int.zero, initType);
        AddRoom(initialRoom);
        GenerateElevator(initialRoom);

        Queue<Room> roomQueue = new Queue<Room>();
        roomQueue.Enqueue(initialRoom);

        int amountToGenerate = Random.Range(roomAmountRange.min, roomAmountRange.max + 1);
        Debug.Log("Max amount rooms to generate: " + amountToGenerate);
        //int elevatorsToGenerate = Random.Range(1, maxElevators + 1);
        //Debug.Log("Max amount elevators to generate: " + elevatorsToGenerate);

        int failCount = 0;
        int maxFails = maxRoomFails; //övregräns för att hindra oändliga loops, när detta uppnås så stoppas generationen.

        while ((rooms.Count < roomAmountRange.max || rooms.Count < roomAmountRange.min) && failCount < maxFails)
        {
            if (roomQueue.Count == 0)
            {
                //Här väljer vi (random) ett nytt room att generera från, görs ifall algoritmen "fastnar".
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
                if (type == RoomType.Elevator)
                    continue;

                MinMaxInt sizeRange = roomSettings[type].sizeRange;

                int width = Random.Range(sizeRange.min, sizeRange.max + 1);
                int height = Random.Range(sizeRange.min, sizeRange.max + 1);

                //Specific settings for corridor (så den blir avlång)
                if (type == RoomType.Corridor)
                {
                    Vector2 dir = GetRandomDirection();
                    bool alongZ = dir == Vector2.up || dir == Vector2.down;
                    width = alongZ
                        ? Random.Range(sizeRange.min, sizeRange.min + 2)
                        : Random.Range(sizeRange.max - 4, sizeRange.max + 1);
                    height = alongZ
                        ? Random.Range(sizeRange.max - 4, sizeRange.max + 1)
                        : Random.Range(sizeRange.min, sizeRange.min + 2);
                }

                Vector2Int direction = GetRandomDirection();
                Vector2Int offset = GetOffset(direction, width, height);
                Vector2Int newPosition = currentRoom.Position + offset;

                Room newRoom = new Room(width, height, wallHeight, wallThickness, doorSize, newPosition, type);

                if (IsRoomSpaceFree(newRoom) && IsRoomConnected(currentRoom, newRoom))
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
        //Extra while-loop för att säkra att hissar skapas. SÄTT EN GRÄNS
        //failCount = 0;
        //while (elevatorsToGenerate > rooms.Count(r => r.Type == RoomType.Elevator) && failCount < maxFails)
        //{
        //    Room currentRoom = rooms[Random.Range(0, rooms.Count)];
        //    //Debug.Log("Attempting elevator");
        //    if (!roomConnectionRules[currentRoom.Type].Contains(RoomType.Elevator))
        //        continue;

        //    bool addedElevatorThisCycle = false;

        //    MinMaxInt sizeRange = roomSettings[RoomType.Elevator].sizeRange;

        //    int width = Random.Range(sizeRange.min, sizeRange.max + 1);
        //    int height = Random.Range(sizeRange.min, sizeRange.max + 1);

        //    Vector2Int direction = GetRandomDirection();
        //    Vector2Int offset = GetOffset(direction, width, height);
        //    Vector2Int newPosition = currentRoom.Position + offset;

        //    Room elevator = new Room(width, height, wallHeight, wallThickness, doorSize, newPosition, RoomType.Elevator);
        //    if (IsRoomSpaceFree(elevator) && IsRoomConnected(elevator, direction))
        //    {
        //        AddRoom(elevator);
        //        addedElevatorThisCycle = true;
        //        Debug.Log("Created elevator");
        //    }

        //    if (!addedElevatorThisCycle)
        //        failCount++;
        //    else
        //        failCount = 0; // Reset fails if successful
        //}
    }

    private void GenerateFurnitureLayout()
    {
        Queue<Furniture> furnitureQueue = new Queue<Furniture>();

        foreach (Room room in rooms)
        {
            //furnitureQueue.Enqueue();

            RoomType type = room.Type;
            int failCount = 0;
            int maxFails = maxFurnitureFails;
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

                if (selectedFurniture.lookAlikeID != 0)
                {
                    if (room.FurnitureList.Any(f => f.Item1.lookAlikeID == selectedFurniture.lookAlikeID))
                    {
                        failCount++;
                        continue;
                    }
                }

                List<Vector2Int> selectedFurnitureTiles = selectedFurniture.GetOccupiedTiles(spawnPos);

                //if (room.Type == RoomType.Elevator)
                //{
                //    if (TryPlaceFurniture(room, selectedFurniture, spawnPos, false, true))
                //    {
                //        Debug.Log("Placed Elevator: " + spawnPos);
                //        addedFurnitureThisCycle = true;
                //        AddFurniture(room, selectedFurniture, spawnPos);
                //    }
                //}
                if (TryPlaceFurniture(room, selectedFurniture, spawnPos))
                {
                    //Debug.Log("Placed furniture: " + spawnPos);
                    addedFurnitureThisCycle = true;
                    AddFurniture(room, selectedFurniture, spawnPos);
                }

                //if (IsFurnitureSpaceFree(room, selectedFurniture, spawnPos))
                //{
                //    selectedFurniture.transform.position = new Vector3(spawnPos.x, 0, spawnPos.y);

                //    bool isClear = true;
                //    foreach (Vector2Int dir in selectedFurniture.clearDirections)
                //    {
                //        //Debug.Log("Amount furniture tiles: " + occupiedFurniturePositions.Count);

                //        bool isRoom = CheckCollidingTilesAtDir(dir, selectedFurnitureTiles, true, roomTiles);
                //        bool isFurniture = CheckCollidingTilesAtDir(dir, selectedFurnitureTiles, false, occupiedFurniturePositions.ToList());

                //        //Debug.Log($"Is dir {dir} a room tile {isRoom} or a furniture tile {isFurniture}");

                //        if (!isRoom || isFurniture)
                //            isClear = false;
                //    }

                //    bool isNextToWall = true;
                //    foreach (Vector2Int dir in selectedFurniture.wallDirections)
                //    {
                //        if (CheckCollidingTilesAtDir(dir, selectedFurnitureTiles, true, roomTiles))
                //            isNextToWall = false;
                //    }

                //    if (isClear && isNextToWall)
                //    {
                //        Debug.Log("Placed furniture: " + spawnPos);
                //        addedFurnitureThisCycle = true;
                //        AddFurniture(room, selectedFurniture, spawnPos);
                //    }
                //    else
                //    {
                //        //Debug.Log("Failed placement");
                //        //AddFurniture(room, selectedFurniture, spawnPos);
                //    }
                //}

                if (!addedFurnitureThisCycle)
                    failCount++;
                else
                    failCount = 0; // Reset fails if successful
            }
        }
    }

    //Hårdkodad lösning för hissen. Just nu hamnar den alltid ovanför det första rummet i mitten och skapas mycket tidigare (för att kringå att det ska ha ett rum osv) men det bryter RoomManagerns struktur
    void GenerateElevator(Room adjacentRoom)
    {
        Vector2Int size = new Vector2Int(2, 2);
        Vector2Int roomMid = adjacentRoom.Position + new Vector2Int(adjacentRoom.Width, adjacentRoom.Height) / 2;
        List<Vector2Int> elevatorTiles = new List<Vector2Int>();

        for (int x = roomMid.x - size.x / 2; x < roomMid.x + size.x / 2; x++)
        {
            for (int y = adjacentRoom.Position.y + adjacentRoom.Height; y < adjacentRoom.Position.y + adjacentRoom.Height + size.y; y++)
            {
                Vector2Int tile = new Vector2Int(x, y);
                elevatorTiles.Add(tile);
                occupiedRoomPositions.Add(tile);
                if (debugger)
                    debugger.furnitureTiles.Add(tile);
            }
        }

        List<Furniture> elevators = roomPrefabManager.GetFurniture(RoomType.Elevator);
        Furniture elevator = elevators[Random.Range(0, elevators.Count())];
        adjacentRoom.FurnitureList.Add((elevator, elevatorTiles[0]));

        //MeshBuilder.CreateFurniture(null, elevator, new Vector3(elevatorTiles[0].x, 0, elevatorTiles[0].y));
    }

    bool TryPlaceFurniture(Room room, Furniture furniture, Vector2Int spawnPos, bool checkDoorTiles = true, bool checkFurnitureTiles = true)
    {
        //Tiles to check
        List<Vector2Int> roomTiles = room.GetOccupiedTiles();
        List<Vector2Int> doorTiles = room.GetDoorTiles(doorClearance);
        List<Vector2Int> furnitureTiles = new List<Vector2Int>();
        foreach ((Furniture f, Vector2Int v) furn in room.FurnitureList)
            furnitureTiles.AddRange(furn.f.GetOccupiedTiles(furn.v));
        
        //TryFurniture tiles
        List<Vector2Int> selectedFurnitureTiles = furniture.GetOccupiedTiles(spawnPos);


        //Kollar så möbeln har utrymme i rummet. Dvs kollar så att den är inom roomTiles och att den inte kolliderar med doorTiles eller furnitureTiles (om inte annorlunda givet i parametrarna)
        bool isSpaceFree = true;
        foreach (Vector2Int tile in selectedFurnitureTiles)
        {
            bool isRoom = CheckCollidingTilesAtDir(Vector2Int.zero, selectedFurnitureTiles, true, roomTiles);
            bool isDoor = checkDoorTiles ? CheckCollidingTilesAtDir(Vector2Int.zero, selectedFurnitureTiles, false, doorTiles) : false;
            bool isFurniture = checkFurnitureTiles ? CheckCollidingTilesAtDir(Vector2Int.zero, selectedFurnitureTiles, false, furnitureTiles) : false;

            if (!isRoom || isDoor || isFurniture)
            {
                isSpaceFree = false;
                return false;
            }
        }

        //Kollar möbelns clearDirections är helt lediga.
        bool isClear = true;
        foreach (Vector2Int dir in furniture.clearDirections)
        {
            //bool isRoom = CheckCollidingTilesAtDir(dir, selectedFurnitureTiles, true, roomTiles);
            //bool isFurniture = CheckCollidingTilesAtDir(dir, selectedFurnitureTiles, false, occupiedFurniturePositions.ToList());

            bool isRoom = CheckCollidingTilesAtDir(dir, selectedFurnitureTiles, true, roomTiles);
            bool isFurniture = checkFurnitureTiles ? CheckCollidingTilesAtDir(dir, selectedFurnitureTiles, false, furnitureTiles) : false;

            if (!isRoom || isFurniture)
            {
                isClear = false;
                return false;
            }
                
        }

        //Kollar möblens wallDirections har repsektive vägg
        bool isNextToWall = true;
        foreach (Vector2Int dir in furniture.wallDirections)
        {
            if (CheckCollidingTilesAtDir(dir, selectedFurnitureTiles, true, roomTiles))
            {
                isNextToWall = false;
                return false;
            }
                
        }

        //extra koll (egentligen onödigt)
        if (isSpaceFree && isClear && isNextToWall)
            return true;
        else
            return false;
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

    //void RoofGeneration()
    //{
    //    MinMaxInt rangeX = new MinMaxInt(0, 0);
    //    MinMaxInt rangeZ = new MinMaxInt(0, 0);

    //    foreach (Room room in rooms)
    //    {
    //        rangeX.min = rangeX.min > room.Position.x ? room.Position.x : rangeX.min;
    //        rangeX.max = rangeX.max < room.Position.x + room.Width ? room.Position.x + room.Width : rangeX.max;

    //        rangeZ.min = rangeZ.min > room.Position.y ? room.Position.y : rangeZ.min;
    //        rangeZ.max = rangeZ.max < room.Position.y + room.Height ? room.Position.y + room.Height : rangeZ.max;
    //    }
    //    Debug.Log(rangeX.min);
    //    Debug.Log(rangeX.max);
    //    Debug.Log(rangeZ.min);
    //    Debug.Log(rangeZ.max);

    //    for (int x = rangeX.min; x <= rangeX.max; x++)
    //    {
    //        for (int z = rangeZ.min; z <= rangeZ.max; z++)
    //        {
    //            Vector2Int tile = new Vector2Int(x, z);
    //            if (occupiedRoomPositions.Contains(tile))
    //                return;

    //            int collidingWalls = 0;

    //            //Right (positiv x)
    //            for (int tileX = x; tileX <= rangeX.max; tileX++)
    //            {
    //                if (occupiedRoomPositions.Contains(new Vector2Int(tileX, tile.y)))
    //                {
    //                    collidingWalls++;
    //                    break;
    //                }
    //            }
    //            //left (negativ x)
    //            for (int tileX = x; tileX >= rangeX.min; tileX--)
    //            {
    //                if (occupiedRoomPositions.Contains(new Vector2Int(tileX, tile.y)))
    //                {
    //                    collidingWalls++;
    //                    break;
    //                }
    //            }
    //            //up (positiv z)
    //            for (int tileY = z; tileY <= rangeZ.max; tileY++)
    //            {
    //                if (occupiedRoomPositions.Contains(new Vector2Int(tile.x, tileY)))
    //                {
    //                    collidingWalls++;
    //                    break;
    //                }
    //            }
    //            //down (negativ z)
    //            for (int tileY = z; tileY >= rangeZ.min; tileY--)
    //            {
    //                if (occupiedRoomPositions.Contains(new Vector2Int(tile.x, tileY)))
    //                {
    //                    collidingWalls++;
    //                    break;
    //                }
    //            }
    //            Debug.Log("Rooftop  amount collided walls: " + collidingWalls);
    //            Debug.Log("Generating rooftop at pos: " + tile);
    //            if (collidingWalls >= 4)
    //            {

    //            }
    //        }
    //    }
    //}

    //Om en grupp av tiles når bordern, då är det inte ett hål annars är det ett hål
    //Vi väljer en tile, förgrenar från den tills den stannar av border/rooms/av tidigare tiles.
    public List<List<Vector2Int>> FindHoles(HashSet<Vector2Int> roomTiles)
    {
        //Width/Height definition
        MinMaxInt rangeX = new MinMaxInt(0, 0);
        MinMaxInt rangeZ = new MinMaxInt(0, 0);

        foreach (Room room in rooms)
        {
            rangeX.min = rangeX.min > room.Position.x ? room.Position.x : rangeX.min;
            rangeX.max = rangeX.max < room.Position.x + room.Width ? room.Position.x + room.Width : rangeX.max;

            rangeZ.min = rangeZ.min > room.Position.y ? room.Position.y : rangeZ.min;
            rangeZ.max = rangeZ.max < room.Position.y + room.Height ? room.Position.y + room.Height : rangeZ.max;
        }

        Vector2Int[] dirs = new[]{ Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down };


        //Definerar bordern
        List<Vector2Int> border = new List<Vector2Int>();
        for (int x = rangeX.min; x <= rangeX.max; x++) //Top/bottom
        {
            border.Add(new Vector2Int(x, rangeZ.min));
            border.Add(new Vector2Int(x, rangeZ.max));
        }
        for (int y = rangeZ.min; y <= rangeZ.max; y++) //Right/Left
        {
            border.Add(new Vector2Int(rangeX.min, y));
            border.Add(new Vector2Int(rangeX.max, y));
        }


        //Kollar varje tile ifall den kan förgrena sig utan att kollidera med bordern (ifall en tile har blivit förgrenad, då räknas den som en del av gruppen och blir skippad i for-loopen
        for (int x = rangeX.min + 2; x <= rangeX.max - 2; x++)
        {
            for (int y = rangeZ.min + 2; y <= rangeZ.max - 2; y++)
            {

            }
        }
    }

    //public List<List<Vector2Int>> FindHoles(HashSet<Vector2Int> roomTiles)
    //{
    //    //Width/Height definition
    //    MinMaxInt rangeX = new MinMaxInt(0, 0);
    //    MinMaxInt rangeZ = new MinMaxInt(0, 0);

    //    foreach (Room room in rooms)
    //    {
    //        rangeX.min = rangeX.min > room.Position.x ? room.Position.x : rangeX.min;
    //        rangeX.max = rangeX.max < room.Position.x + room.Width ? room.Position.x + room.Width : rangeX.max;

    //        rangeZ.min = rangeZ.min > room.Position.y ? room.Position.y : rangeZ.min;
    //        rangeZ.max = rangeZ.max < room.Position.y + room.Height ? room.Position.y + room.Height : rangeZ.max;
    //    }

    //    int W = rangeX.max - rangeX.min + 2;
    //    int H = rangeZ.max - rangeZ.min + 2;

    //    // 1) occupied grid
    //    bool[,] occupied = new bool[W, H];
    //    foreach (Vector2Int tile in roomTiles)
    //    {
    //        Debug.Log("Global tile: " + tile);
    //        Debug.Log("Local tile; " + new Vector2Int(tile.x - rangeX.min, tile.y - rangeZ.min));
    //        Debug.Log($"Width {W}, Height {H}");
    //        Debug.Log("");
    //        occupied[tile.x - rangeX.min, tile.y - rangeZ.min] = true;
    //    }

    //    // 2) exterior flood
    //    bool[,] exterior = new bool[W, H];
    //    Queue<Vector2Int> queue = new Queue<Vector2Int>();
    //    void TryEnqueue(int x, int y)
    //    {
    //        if (x < 0 || x >= W || y < 0 || y >= H) return;
    //        if (occupied[x, y] || exterior[x, y]) return;

    //        exterior[x, y] = true;
    //        queue.Enqueue(new Vector2Int(x + rangeX.min, y + rangeZ.min));
    //    }
    //    // seed borders
    //    for (int x = 0; x < W; x++)
    //    {
    //        TryEnqueue(x, 0); 
    //        TryEnqueue(x, H - 1);
    //    }
    //    for (int y = 0; y < H; y++)
    //    {
    //        TryEnqueue(0, y); 
    //        TryEnqueue(W - 1, y);
    //    }
    //    // BFS
    //    Vector2Int[] dirs = new[]{ Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down };
    //    while (queue.Count > 0)
    //    {
    //        Vector2Int vector = queue.Dequeue();
    //        foreach (Vector2Int dir in dirs) TryEnqueue(vector.x + dir.x, vector.y + dir.y);
    //    }

    //    // 3) collect hole‐tiles
    //    bool[,] used = new bool[W, H];
    //    List<List<Vector2Int>> holes = new List<List<Vector2Int>>();
    //    for (int x = 0; x < W; x++)
    //    {
    //        for (int y = 0; y < H; y++)
    //        {
    //            if (occupied[x, y] || exterior[x, y] || used[x, y]) continue;
    //            // new hole region!
    //            List<Vector2Int> region = new List<Vector2Int>();
    //            Queue<Vector2Int> rq = new Queue<Vector2Int>();

    //            used[x, y] = true;

    //            rq.Enqueue(new Vector2Int(x + rangeX.min, y + rangeZ.min));

    //            while (rq.Count > 0)
    //            {
    //                Vector2Int vector = rq.Dequeue();
    //                region.Add(vector);
    //                foreach (Vector2Int dir in dirs)
    //                {
    //                    Vector2Int newHole = vector + dir;

    //                    if (newHole.x < 0 || newHole.x >= W || newHole.y < 0 || newHole.y >= H) continue;
    //                    if (occupied[newHole.x, newHole.y] || exterior[newHole.x, newHole.y] || used[newHole.x, newHole.y]) continue;

    //                    used[newHole.x, newHole.y] = true;
    //                    rq.Enqueue(newHole + new Vector2Int(rangeX.min, rangeZ.min));
    //                }
    //            }

    //            holes.Add(region);
    //        }
    //    }

    //    return holes;
    //}

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

    bool IsRoomConnected(Room previousRoom, Room newRoom)
    {
        int minOverlap = Mathf.Max(Mathf.CeilToInt(previousRoom.DoorSize), Mathf.CeilToInt(newRoom.DoorSize));

        bool adjacentHorizontally =
            newRoom.Position.x + newRoom.Width == previousRoom.Position.x ||
            newRoom.Position.x - previousRoom.Width == previousRoom.Position.x;
        if (adjacentHorizontally)
        {
            int top = Mathf.Min(newRoom.Position.y + newRoom.Height, previousRoom.Position.y + previousRoom.Height);
            int bottom = Mathf.Max(newRoom.Position.y, previousRoom.Position.y);    
            int overlapY = top - bottom;
            return overlapY >= minOverlap;
        }

        bool adjacentVertically =
            newRoom.Position.y + newRoom.Height == previousRoom.Position.y ||
            newRoom.Position.y - previousRoom.Height == previousRoom.Position.y;
        if (adjacentVertically)
        {
            int right = Mathf.Min(newRoom.Position.x + newRoom.Width, previousRoom.Position.x + previousRoom.Width);
            int left = Mathf.Max(newRoom.Position.x, previousRoom.Position.x);
            int overlapX = right - left;
            return overlapX >= minOverlap;
        }

        return false;
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
        room.FurnitureList.Add((furniture, pos));

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
