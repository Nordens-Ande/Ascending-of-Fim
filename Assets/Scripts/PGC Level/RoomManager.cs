using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [SerializeField] Material m_Material;
    private List<Room> rooms;

    void Start()
    {
        rooms = new List<Room>();
        rooms.Add(new Room(4, 2, 1, 0.1f, Vector2Int.zero));
        rooms.Add(new Room(4, 2, 1, 0.1f, new Vector2Int(0, 2)));

        CheckNearbyRooms();

        GameObject roomObject1 = MeshBuilder.CreateRoomMesh(rooms[0], m_Material, m_Material);
        GameObject roomObject2 = MeshBuilder.CreateRoomMesh(rooms[1], m_Material, m_Material);
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

                    Vector2Int door1 = new Vector2Int(bounds1.xMax - doorX, bounds1.yMax);
                    Vector2Int door2 = new Vector2Int(bounds2.xMin + doorX, bounds1.yMin);

                    rooms[i].Doorways.Add(door1);
                    rooms[j].Doorways.Add(door2);

                    Debug.Log("Created Doorways");
                    Debug.Log(door1);
                    Debug.Log(door2);
                }
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
