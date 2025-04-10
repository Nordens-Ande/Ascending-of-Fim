using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [SerializeField] Material m_Material;
    private List<Room> rooms;

    void Start()
    {
        rooms = new List<Room>();
        rooms.Add(new Room(6, 2, 1, 0.1f, new Vector2Int(0, 0)));
        rooms.Add(new Room(6, 2, 1, 0.1f, new Vector2Int(0, 2)));
        rooms.Add(new Room(6, 2, 1, 0.1f, new Vector2Int(0, 4)));

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
                BoundsInt bounds1 = rooms[i].GetBounds();
                BoundsInt bounds2 = rooms[j].GetBounds();

                //Top/front dörr
                if (bounds1.yMax == bounds2.yMin && bounds1.xMax > bounds2.xMin)
                {
                    int range = bounds1.xMax - bounds2.xMin;
                    int doorX = Random.Range(0, range);

                    Vector2Int door1 = new Vector2Int(rooms[j].Position.x - rooms[i].Position.x + doorX, bounds1.yMax - bounds1.yMin);
                    Vector2Int door2 = new Vector2Int(bounds2.xMin + doorX, bounds1.yMin - bounds1.yMin);

                    rooms[i].Doorways.Add(door1);
                    rooms[j].Doorways.Add(door2);

                    Debug.Log("Created Doorways");
                    Debug.Log($"Range: {range} and randomX: {doorX}");
                    Debug.Log($"Doorway for room1: {door1} max as: {bounds1.xMax - 0} and min as: {bounds1.xMax - range} and posX: {rooms[i].Position.x}");
                    Debug.Log($"Doorway for room2: {door2} max as: {bounds2.xMin + range} and min as: {bounds1.xMin + 0} and posX: {rooms[j].Position.x}");
                }
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
