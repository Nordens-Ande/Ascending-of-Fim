using UnityEngine;

public class SimpleRoomBuilder : MonoBehaviour
{
    [SerializeField] int Width, Height;
    [SerializeField] Vector2Int Position;
    [SerializeField] float WallHeight, WallThickness;
    [SerializeField] float DoorSize;
    [SerializeField] Material material;

    private int currWidth, currHeight;
    private Vector2Int currPosition;
    private float currWallHeight, currWallThickness;
    private Material currMaterial;

    private Room room;
    private GameObject roomObject;

    private void CreateNewRoom()
    {
        if (roomObject != null)
            Destroy(roomObject);

        room = new Room(Width, Height, WallHeight, WallThickness, DoorSize, Position);
        room.Doorways.Add(new Vector2Int(1, 1));
        roomObject = MeshBuilder.CreateRoomMesh(room, material, material);

        currWidth = Width;
        currHeight = Height;
        currPosition = new Vector2Int(Position.x, Position.y);
    }

    void Start()
    {
        if (room == null || roomObject == null)
            CreateNewRoom();
    }

    // Update is called once per frame
    void Update()
    {
        if (roomObject != null)
        {
            bool roomChanged = room.Equals(Width, Height, WallHeight, WallThickness, Position, room.Doorways);
            if (roomChanged || material != currMaterial)
                CreateNewRoom();
        }
    }
}
