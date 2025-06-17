using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

//En helper struct, som ger en minimum och maximum integer värden, denna struct blir användbar då det används ofta genom denna klassen.
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

//En struct vars syfte är att underlätta konfigurationen av lägenheter, RoomSettings innehåller alla relevanta rum specifika parametrar och gör det snyggare i inspectorn
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

//Denna klassen är ansvarig för att generera lägenheterna och hela dess interiör, dvs dörrar, möbler, mesh, spawnpoints mm.
//För att skapa allting så använder jag mig av ett tile system för att kolla ifall området är redan ockuperat. Dvs alla möbler, dörrar och rum är int stora och kan bara följa int rutnätet.
public class RoomManager : MonoBehaviour
{
    [SerializeField] public bool reroll = false; //Denna bool används för att snabbt kunna generera en ny lägenhet

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
    [SerializeField] public MinMaxInt roomAmountRange;
    [SerializeField] public int floorLevel = 1;
    [SerializeField] Material exteriorWallMaterial;
    [Space]
    [SerializeField] int doorClearance = 1;
    [Space]
    [SerializeField] bool thickWallsEnabled = true;
    [SerializeField] Material thickWallMat;
    [SerializeField] float thickWallHeightOffset;
    [Space]
    [SerializeField] int maxRoomFails = 1000;
    [SerializeField] int maxFurnitureFails = 100;

    [Header("Keycard Spawn Settings")]
    [SerializeField] Furniture keycard;
    [Space]
    [SerializeField] int maxKeycards;
    [SerializeField] int maxKeycardsPerRoom;
    [SerializeField] List<RoomType> roomsToExcludeKeycard;

    [Header("Enemy Spawnpoint Settings")]
    [SerializeField] Furniture enemySpawnPoint;
    [Space]
    [SerializeField] int maxEnemySpawnpoints;
    [SerializeField] int maxEnemySpawnpointsPerRoom;
    [SerializeField] int maxEnemySpawnpointsFails = 1500;
    [SerializeField] List<RoomType> roomsToExcludeEnemies;

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
    private Dictionary<Room, GameObject> roomDictionary;
    private GameObject thickWalls;
    private GameObject exteriorWalls;

    private List<GameObject> furnitureObjects;

    //tiles för olika typer i lägenhetern, room, door, furniture, walls
    private HashSet<Vector2Int> occupiedRoomPositions;
    private HashSet<Vector2Int> occupiedDoorPositions;
    private HashSet<Vector2Int> occupiedFurniturePositions;
    private HashSet<Vector2Int> occupiedThickWalls;

    private Dictionary<RoomType, MinMaxInt> roomSizes;
    private Dictionary<RoomType, RoomSettings> roomSettings;

    //Här definerar vi reglerna för vilka rum som kan kopplas ihop
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
        //Bestämmer room settings
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

    //Start metoden generar en helt ny lägenhet. Den skapar då alla meshes, rum, möbler, spawnpoints osv. När vi gör en reroll, dvs genererar om då kallas start igen - eftersom att den innehåller allt nödvändigt för att skapa en lägenhet
    //Konstruktionen av lägenheten fungerar så att vi generar först all information, var rummen är, dörrar, möbler, spawnpoints osv och när det är färdigt då börjar vi bygga upp alla meshes
    //Detta görs delvis för att koden är gjord att vara snabb och effektiv, vi behöver inte skapa meshen på rummen för att skapa dörrar eller möbler vilket drastiskt gör kosntruktionen av lägenheter snabbare
    void Start()
    {
        reroll = false;
        if (player)
        {
            player.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            player.transform.position = new Vector3(initWidth / 2f, 3, initHeight / 2f);
            player.transform.position = new Vector3(initWidth / 2f, 3, initHeight / 2f);
        }
            

        rooms = new List<Room>();
        roomObjects = new List<GameObject>();
        roomDictionary = new Dictionary<Room, GameObject>();

        occupiedRoomPositions = new HashSet<Vector2Int>();
        occupiedDoorPositions = new HashSet<Vector2Int>();
        occupiedFurniturePositions = new HashSet<Vector2Int>();
        occupiedThickWalls = new HashSet<Vector2Int>();


        GenerateRoomLayout(); //Rooms

        if (thickWallsEnabled) //Thick walls
            ThickWallsGeneration();

        CheckNearbyRooms(); //Doors

        GenerateFurnitureLayout(); //Furniture
        MustPlaceFurniture(keycard, maxKeycards, maxKeycardsPerRoom, roomsToExcludeKeycard.ToArray()); //Keycard

        maxEnemySpawnpoints = roomAmountRange.min;
        MustPlaceFurniture(enemySpawnPoint, maxEnemySpawnpoints, maxEnemySpawnpointsPerRoom, maxEnemySpawnpointsFails, roomsToExcludeKeycard.ToArray()); //Enemy spawnpoints

        DebugGeneration(); //Debug tiles

        //Meshbuilder
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
        if (thickWallsEnabled)
        {
            thickWalls = new GameObject();
            thickWalls.name = "Thick Wall Tiles";
            Material mat = thickWallMat ? thickWallMat : wallMaterial;
            foreach (Vector2Int tile in occupiedThickWalls)
            {
                MeshBuilder.CreateThickWallTile(thickWalls.transform, tile, wallHeight + thickWallHeightOffset, mat);
            }
        }
        exteriorWalls = new GameObject();
        exteriorWalls.name = "Exterior walls";
        MeshBuilder.CreateExteriorWalls(exteriorWalls.transform, occupiedRoomPositions.Concat(occupiedThickWalls).ToHashSet(), wallHeight, wallHeight * floorLevel, wallThickness, exteriorWallMaterial);

        //Enemies
        if(navMeshBaker != null)
        {
            StartCoroutine(navMeshBaker.BakeNavMesh());
        }

        if(enemySpawnPointManager != null)
        {
            enemySpawnPointManager.GetSpawnPoints();
        }
    }

    //Denna metod skapar en while loop som försöker generera angivna talet på rum. Här finns det säkerhets spärrar så den inte fastnar i en evig while- loop
    //Denna metoden fungerar genom att skapa ett rum och sen skapa ett nytt rum från det rummet och växer vidare så. Om den stannar (kan inte göra fler rum) då väljer den ett nytt rum som den ska växa ifrån
    //Då lyder dessa rum våra tidigare angivna krav/regler/inställningar, dvs vilket rum som kan kopplas, hur stort det är osv.
    private void GenerateRoomLayout()
    {
        //Skapar det första rummet och hissen som alla andra rum kommer utgå från.
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

            //Här går vi genom alla rum som kan skapas och som inte bryter reglerna i possibleConnections
            foreach (RoomType type in possibleConnections)
            {
                //Kontroll ifall vi har uppnåt max rum eller om det är en hiss som vi försöker skapa
                if (rooms.Count >= amountToGenerate)
                    break;
                if (type == RoomType.Elevator)
                    continue;

                //Storleken på rummet
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

                //Position för det nya rummet
                Vector2Int direction = GetRandomDirection();
                Vector2Int offset = GetOffset(direction, width, height);
                Vector2Int newPosition = currentRoom.Position + offset;

                //Skapar nya rummet
                Room newRoom = new Room(width, height, wallHeight, wallThickness, doorSize, newPosition, type);

                //Kontrollerar så att vårt nya rum ej kolliderar med andra rum och att det är dessutom kopplat till vårt tidigare rum som vi bygger ifrån
                if (IsRoomSpaceFree(newRoom) && IsRoomConnected(currentRoom, newRoom))
                {
                    AddRoom(newRoom);
                    roomQueue.Enqueue(newRoom);
                    addedRoomThisCycle = true;
                }
            }

            if (!addedRoomThisCycle)
                failCount++; //Adder på vår fail-safe int
            else
                failCount = 0; //Resetar ifall den lyckates skapa ett rum
        }
    }

    //Denna metoden genrerar alla möbler för alla rum - och kallas då efter vi har skapat alla rummen (inte nödvändigtvis meshen)
    //Denna metoden är också en while-loop och har även säkerhetsspärrar för att hindra att programmet fryser
    private void GenerateFurnitureLayout()
    {
        Queue<Furniture> furnitureQueue = new Queue<Furniture>();

        //Går igenom alla rummen och skapar möbler
        foreach (Room room in rooms)
        {
            //furnitureQueue.Enqueue();

            RoomType type = room.Type;
            int failCount = 0;
            int maxFails = maxFurnitureFails;
            int amountToGenerate = Random.Range(1, roomSettings[type].maxFurniture + 1); //bestämmer antalet möbler

            while (failCount < maxFails && room.FurnitureList.Count <= roomSettings[type].maxFurniture)
            {
                if (room.FurnitureList.Count >= amountToGenerate) //Extra check ifall vi har uppnått max möbler redan
                    break;

                //Hämtar och kontrollerar att möblerna finns och fungerar - annars fortsätter den tills den misslyckas (failCount uppnås)
                List<Furniture> availableFurnitures = roomPrefabManager.GetFurniture(room.Type);
                if (availableFurnitures == null || availableFurnitures.Count == 0)
                {
                    failCount++;
                    continue;
                }
                    
                bool addedFurnitureThisCycle = false;

                //Hämtar tiles för dörrar och rum
                List<Vector2Int> roomTiles = room.GetOccupiedTiles();
                List<Vector2Int> doorTiles = room.GetDoorTiles(doorClearance);
                //List<Vector2Int> furnitureTiles = new List<Vector2Int>();
                //foreach (Furniture furniture in room.FurnitureList.Keys)
                //    furnitureTiles.AddRange(furniture.GetOccupiedTiles(room.FurnitureList[furniture]));

                //väljer en slumpmässig plats i rummet som vi sedan testar ifall vi kan generara en slumpmässig möbel på
                Vector2Int spawnPos = roomTiles[Random.Range(0, roomTiles.Count)];
                Furniture selectedFurniture = availableFurnitures[Random.Range(0, availableFurnitures.Count)];

                //Kontroll för att kolla så samma möbel (eller snarklik med ID) redan finns i rummet
                if (selectedFurniture.lookAlikeID != 0)
                {
                    if (room.FurnitureList.Any(f => f.Item1.lookAlikeID == selectedFurniture.lookAlikeID))
                    {
                        failCount++;
                        continue;
                    }
                }

                //Hämtar tiles var möblen ockuperar
                List<Vector2Int> selectedFurnitureTiles = selectedFurniture.GetOccupiedTiles(spawnPos);

                //Försöker att placera möbeln 
                if (TryPlaceFurniture(room, selectedFurniture, spawnPos))
                {
                    //Debug.Log("Placed furniture: " + spawnPos);
                    addedFurnitureThisCycle = true;
                    AddFurniture(room, selectedFurniture, spawnPos);
                }

                if (!addedFurnitureThisCycle)
                    failCount++;
                else
                    failCount = 0;
            }
        }
    }

    //Hårdkodad lösning för hissen. Just nu hamnar den alltid ovanför det första rummet i mitten och skapas mycket tidigare (för att kringå att det ska ha ett rum osv) men det bryter RoomManagerns struktur.
    //Detta fungerar tillräckligt bra för spelet och gör hissen kännas mer rimlig då den alltid befinner sig på samma plats i lägenheten
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

    //Must som innebär att den måste placera den angivna möbeln oavsett vad - här så ignorerar den vilket rum det är och bryr sig enbart om max mängd i lägenheten/rummen och ifall den ska undvika att placeras i specifika rum.
    //Denna används primärt för keycard
    void MustPlaceFurniture(Furniture furniture, int maxAmount, int maxPerRoom = 1, params RoomType[] excludedRooms)
    {
        if (furniture == null) return;

        int placedFurniture = 0;
        while (placedFurniture < maxAmount)
        {
            Room room = rooms[Random.Range(0, rooms.Count)];

            //Kollar ifall det är ett rum vi ska ignorera (exkludera)
            bool isExcluded = false;
            foreach (RoomType roomType in excludedRooms)
            {
                if (room.Type == roomType)
                {
                    isExcluded = true;
                    break;
                }
            }
            if (isExcluded) continue;

            //Kollar ifall rummet vi har valt har för många möbler
            int placedFurniturePerRoom = 0;
            foreach ((Furniture f, Vector2Int v) checkFurniture in room.FurnitureList)
            {
                if (furniture == checkFurniture.f)
                    placedFurniturePerRoom++;
            }
            if (placedFurniturePerRoom >= maxPerRoom) continue;


            List<Vector2Int> roomTiles = room.GetOccupiedTiles();
            Vector2Int spawnPos = roomTiles[Random.Range(0, roomTiles.Count)];

            if (TryPlaceFurniture(room, furniture, spawnPos))
            {
                room.FurnitureList.Add((furniture, spawnPos));
                Debug.Log("Spawned keycard at: " + spawnPos);
                placedFurniture++;
            }
        }
    }

    //Samma som förra metod fast här har den en övregräns (maxFails) för att undvika så att den inte fastnar i en while-loop
    //Denna används primärt för fienderna så att spawnpoints hamnar i alla rum
    void MustPlaceFurniture(Furniture furniture, int maxAmount, int maxPerRoom, int maxFails, params RoomType[] excludedRooms)
    {
        if (furniture == null) return;

        int placedFurniture = 0;
        int failCount = 0;
        while (placedFurniture < maxAmount && failCount < maxFails)
        {
            Room room = rooms[Random.Range(0, rooms.Count)];

            //Kollar ifall det är ett rum vi ska ignorera (exkludera)
            bool isExcluded = false;
            foreach (RoomType roomType in excludedRooms)
            {
                if (room.Type == roomType)
                {
                    isExcluded = true;
                    break;
                }
            }
            if (isExcluded) continue;

            //Kollar ifall rummet vi har valt har för många möbler
            int placedFurniturePerRoom = 0;
            foreach ((Furniture f, Vector2Int v) checkFurniture in room.FurnitureList)
            {
                if (furniture == checkFurniture.f)
                    placedFurniturePerRoom++;
            }
            if (placedFurniturePerRoom >= maxPerRoom) continue;


            List<Vector2Int> roomTiles = room.GetOccupiedTiles();
            Vector2Int spawnPos = roomTiles[Random.Range(0, roomTiles.Count)];

            if (TryPlaceFurniture(room, furniture, spawnPos))
            {
                room.FurnitureList.Add((furniture, spawnPos));
                //Debug.Log("Spawned keycard at: " + spawnPos);
                failCount = 0;
                placedFurniture++;
            }
        }
    }

    //Metoden som kollar ifall den givna möbeln får plats i rummet utan att vara utanför, i dörren eller i en annan möbel. Vi kan ange också ifall vi vill kontrollera doorTiles och funritureTiles
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

    //Metod för att skapa dörrar mellan alla rum
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

    //Försöker skapa en dörr mellan rum A och B. Väljer sedan en slumpmässig plats där den placerar dörren på vägen
    void TryCreateDoorsBetween(Room roomA, Room roomB)
    {
        BoundsInt boundsA = roomA.GetBounds();
        BoundsInt boundsB = roomB.GetBounds();

        float doorSize = roomA.DoorSize;

        // FRONT/DOWN & BACK/UP (A är äver B)
        if (boundsA.yMin == boundsB.yMax)
        {
            int overlapMinX = Mathf.Max(boundsA.xMin, boundsB.xMin);
            int overlapMaxX = Mathf.Min(boundsA.xMax, boundsB.xMax);
            float overlap = overlapMaxX - overlapMinX;

            //Här kontrollerar vi att dörren faktikt får plats
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

            //Här kontrollerar vi att dörren faktikt får plats
            if (overlap >= doorSize)
            {
                int doorZ = Mathf.RoundToInt(Random.Range(overlapMinY + doorSize / 2f, overlapMaxY - doorSize / 2f));
                roomA.Doorways.Add(new Vector2(0, doorZ - boundsA.yMin)); // Left (A)
                roomB.Doorways.Add(new Vector2(roomB.Width, doorZ - boundsB.yMin)); // Right vägg (B)
            }
        }

        AddDoorSpace(roomA);
    }

    void ThickWallsGeneration()
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

        //Definerar tillgängliga tiles
        List<Vector2Int> freeTiles = new List<Vector2Int>();
        for (int x = rangeX.min; x <= rangeX.max; x++)
        {
            for (int y = rangeZ.min; y <= rangeZ.max; y++)
            {
                Vector2Int tile = new Vector2Int(x, y);
                if (!occupiedRoomPositions.Contains(tile))
                    freeTiles.Add(tile);
            }
        }

        foreach (Vector2Int tile in freeTiles)
        {
            if (IsThickWall(tile, occupiedRoomPositions.ToList()))
                occupiedThickWalls.Add(tile);
        }

        foreach (Vector2Int tile in FindHoles(occupiedRoomPositions))
        {
            occupiedThickWalls.Add(tile);
        }
    }

    //Denna metoden används för att fylla ut hål (donut hål) som bildas i lägenheten. Detta är enbart kosmetiskt men koden/logiken är ändå ganska komplex
    //Om en grupp av tiles når bordern, då är det inte ett hål annars är det ett hål
    //Vi väljer en tile, förgrenar från den tills den stannar av border/rooms/av tidigare tiles.
    public List<Vector2Int> FindHoles(HashSet<Vector2Int> roomTiles)
    {
        List<Vector2Int> holeTiles = new List<Vector2Int>();

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

        //Definerar bordern
        HashSet<Vector2Int> border = new HashSet<Vector2Int>();
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
        //foreach (Vector2Int tile in border)
        //    debugger.doorTiles.Add(tile);
        if (debugger)
            debugger.freeTiles.Add((border, Color.red));


        //Definerar tillgängliga tiles
        List<Vector2Int> freeTiles = new List<Vector2Int>();
        for (int x = rangeX.min + 2; x <= rangeX.max - 2; x++)
        {
            for (int y = rangeZ.min + 2; y <= rangeZ.max - 2; y++)
            {
                Vector2Int tile = new Vector2Int(x, y);
                if (!roomTiles.Contains(tile) || !occupiedThickWalls.Contains(tile))
                    freeTiles.Add(tile);
            }
        }

        int freeTilesIndex = 0;
        while (freeTilesIndex < freeTiles.Count())
        {
            Vector2Int origoTile = freeTiles[freeTilesIndex];
            if (roomTiles.Contains(origoTile) || debugger.holeTiles.Contains(origoTile))
            {
                freeTilesIndex++;
                continue;
            }

            int freeTile = 1;

            List<Vector2Int> grownTiles = new List<Vector2Int>() { origoTile };
            List<Vector2Int> latestTiles = new List<Vector2Int>() { origoTile };

            bool hasTouchedBorder = false;

            //Free tile syftar på antalet nya tiles den kan/har genererat. När den slutar växa når noll då kollar vi ifall den har nuddat gränsen
            while (freeTile > 0)
            {
                List<Vector2Int> copyLatestTiles = new List<Vector2Int>(latestTiles);
                latestTiles.Clear();
                freeTile = 0;

                foreach (Vector2Int tile in copyLatestTiles)
                {
                    List<Vector2Int> grown = GrowTile(tile, grownTiles, roomTiles.ToList());
                    freeTile += grown.Count();

                    foreach (Vector2Int grownTile in grown)
                    {
                        grownTiles.Add(grownTile);
                        latestTiles.Add(grownTile);
                        freeTiles.Remove(grownTile);

                        if (border.Contains(grownTile))
                            hasTouchedBorder = true;
                    }
                }

                if (hasTouchedBorder)
                    break;
            }

            //Ifall växten av tilesen aldrig lyckas nudda gränsen av lägenheten då måste det vara ett slutet hål som kan fyllas ut
            if (!hasTouchedBorder)
            {
                foreach (Vector2Int tile in grownTiles)
                {
                    //Debug.Log(freeTilesIndex + " " + tile);
                    holeTiles.Add(tile);
                    //debugger.holeTiles.Add(tile);
                }
            }
            else
            {
                Color randomColor = Random.ColorHSV();
                HashSet<Vector2Int> tileSet = new HashSet<Vector2Int>(grownTiles);
                debugger.freeTiles.Add((tileSet, randomColor));
            }

            freeTilesIndex++;
        }

        return holeTiles;
    }

    //Metod som ger tiles kring området av tile.
    List<Vector2Int> GrowTile(Vector2Int tile, List<Vector2Int> grownTiles, List<Vector2Int> roomTiles)
    {
        Vector2Int[] dirs = new[] { Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down };
        List<Vector2Int> result = new List<Vector2Int>();

        foreach (Vector2Int dir in dirs)
        {
            Vector2Int newTile = tile + dir;
            bool isRoom = roomTiles.Contains(newTile);
            bool isGrown = grownTiles.Contains(newTile);
            bool isThickWall = occupiedThickWalls.Contains(newTile);

            if (!isRoom && !isGrown && !isThickWall)
            {
                //Debug.Log("Added grown tile: " + newTile);
                result.Add(newTile);
            }
        }

        return result;
    }

    
    bool IsThickWall(Vector2Int tile, List<Vector2Int> roomTiles)
    {
        Vector2Int[] vertical = new[] { Vector2Int.up, Vector2Int.down };
        Vector2Int[] horizontal = new[] { Vector2Int.right, Vector2Int.left };

        bool isVertical = true;
        foreach (Vector2Int dir in vertical)
        {
            if (!roomTiles.Contains(tile + dir)) 
                isVertical = false;
        }

        bool isHorizontal = true;
        foreach(Vector2Int dir in horizontal)
        {
            if (!roomTiles.Contains(tile + dir))
                isHorizontal = false;
        }

        return isVertical || isHorizontal;
    }

    //Genererar all debug information
    void DebugGeneration()
    {
        if (!debugger)
            return;

        debugger.roomTiles = new HashSet<Vector2Int>(occupiedRoomPositions);
        debugger.doorTiles = new HashSet<Vector2Int>(occupiedDoorPositions);
        debugger.furnitureTiles = new HashSet<Vector2Int>(occupiedFurniturePositions);
        debugger.holeTiles = new HashSet<Vector2Int>(occupiedThickWalls);
    }


    //REFERNS (FÖRSTA FÖRSÖK PÅ FURNITURE, IGNORERA DETTA)
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

    //Kollar ifall tiles kolliderar med tiles åt en given riktning. Här kan vi bestämma ifall alla tiles eller bara en måste kollidera ifall det ska bli true (allMustCollide)
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

    //Metod för att kolla så att rum utrymmet är ledigt
    bool IsRoomSpaceFree(Room room)
    {
        foreach (Vector2Int tile in room.GetOccupiedTiles())
        {
            if (occupiedRoomPositions.Contains(tile))
                return false;
        }
        return true;
    }

    //Metod för att kolla så möbeln får plats - denna används inte längre
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

    //Kollar så att rummet har placerats rätt och är kopplat ihop med rummet (var en okänd bug som gjorde att rum hamnade längre bort - denna metoden löser det)
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

    //Lägger till rummet och dess tiles
    void AddRoom(Room room)
    {
        rooms.Add(room);
        foreach (Vector2Int tile in room.GetOccupiedTiles())
        {
            occupiedRoomPositions.Add(tile);
        }
    }

    //Lägger till möbeln och dess tiles
    void AddFurniture(Room room, Furniture furniture, Vector2Int pos)
    {
        room.FurnitureList.Add((furniture, pos));

        foreach (Vector2Int tile in furniture.GetOccupiedTiles())
        {
            occupiedFurniturePositions.Add(pos + tile);
        }
    }
    
    //Lägger till dörren och dess tiles
    void AddDoorSpace(Room room)
    {
        foreach (Vector2Int tile in room.GetDoorTiles(doorClearance))
        {
            occupiedRoomPositions.Add(tile);
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

    //Update ifall vi har rerollat lägenheten.
    private void FixedUpdate()
    {
        if (reroll)
        {
            reroll = false;

            foreach (GameObject room in roomObjects)
                Destroy(room);
            Destroy(thickWalls);
            Destroy(exteriorWalls);

            if (debugger)
                debugger.ClearTiles();

            Awake();

            Start();
        }
    }
}
