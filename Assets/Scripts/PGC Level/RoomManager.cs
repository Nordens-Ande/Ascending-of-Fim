using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class RoomManager : MonoBehaviour
{
    [SerializeField] Material m_Material;
    private List<Room> rooms;

    void Start()
    {
        rooms = new List<Room>();
        //rooms.Add(new Room(6, 2, 1, 0.1f, 1.25f, new Vector2Int(0, 0)));
        rooms.Add(new Room(6, 2, 1, 0.1f, 1.25f, new Vector2Int(0, 2)));
        rooms.Add(new Room(6, 2, 1, 0.1f, 1.25f, new Vector2Int(2, 4)));
        rooms.Add(new Room(4, 2, 1, 0.1f, 1.25f, new Vector2Int(6, 2)));

        CheckNearbyRooms();

        GameObject roomObject1 = MeshBuilder.CreateRoomMesh(rooms[0], m_Material, m_Material);
        GameObject roomObject2 = MeshBuilder.CreateRoomMesh(rooms[1], m_Material, m_Material);
        GameObject roomObject3 = MeshBuilder.CreateRoomMesh(rooms[2], m_Material, m_Material);
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
                float doorX = Random.Range(overlapMinX + doorSize / 2f, overlapMaxX - doorSize / 2f);
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
                float doorZ = Random.Range(overlapMinY + doorSize / 2f, overlapMaxY - doorSize / 2f);
                roomA.Doorways.Add(new Vector2(0, doorZ - boundsA.yMin)); // Left (A)
                roomB.Doorways.Add(new Vector2(roomB.Width, doorZ - boundsB.yMin)); // Right vägg (B)
            }
        }
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
