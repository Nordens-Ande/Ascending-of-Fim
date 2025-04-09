using UnityEngine;

public static class MeshBuilder
{
    public static GameObject CreateRoomMesh(Room room, Material wallMat, Material floorMat)
    {
        //Skapar GameObject-et
        GameObject root = new GameObject($"Room_{room.Position}");

        Vector3 origin = new Vector3(room.Position.x, 0, room.Position.y);
        Vector3 size = new Vector3(room.Width, 0, room.Height);

        //Definerar golv
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.transform.parent = root.transform;
        floor.transform.position = origin + size / 2f;
        floor.transform.localScale = new Vector3(size.x, 0.1f, size.z);
        floor.GetComponent<Renderer>().material = floorMat;

        //Definar fyra vägar (4 directions)
        Vector3 wallScaleX = new Vector3(room.WallThickness, room.WallHeight, room.Height);
        Vector3 wallScaleZ = new Vector3(room.Width, room.WallHeight, room.WallThickness);

        //CreateWall(root.transform, origin + new Vector3(0, room.WallHeight / 2f, room.Height / 2f), wallScaleX, wallMat);            //Vänster
        //CreateWall(root.transform, origin + new Vector3(room.Width, room.WallHeight / 2f, room.Height / 2f), wallScaleX, wallMat);   //Höger
        //CreateWall(root.transform, origin + new Vector3(room.Width / 2f, room.WallHeight / 2f, 0), wallScaleZ, wallMat);             //Fram
        //CreateWall(root.transform, origin + new Vector3(room.Width / 2f, room.WallHeight / 2f, room.Height), wallScaleZ, wallMat);   //Bak

        for (int x = 0; x < room.Width; x++)
        {
            //Front vägg (z = 0)
            if (!room.Doorways.Contains(new Vector2Int(x, 0)))
                CreateWall(root.transform, origin + new Vector3(x + 0.5f, room.WallHeight / 2f, 0), new Vector3(1, room.WallHeight, room.WallThickness), wallMat);

            //Back vägg (z = height)
            if (!room.Doorways.Contains(new Vector2Int(x, room.Height)))
                CreateWall(root.transform, origin + new Vector3(x + 0.5f, room.WallHeight / 2f, room.Height), new Vector3(1, room.WallHeight, room.WallThickness), wallMat);
        }

        for (int z = 0; z < room.Height; z++)
        {
            //Vänster vägg (x = 0)
            if (!room.Doorways.Contains(new Vector2Int(0, z)))
                CreateWall(root.transform, origin + new Vector3(0, room.WallHeight / 2f, z + 0.5f), new Vector3(room.WallThickness, room.WallHeight, 1), wallMat);

            //Höger vägg (x = width)
            if (!room.Doorways.Contains(new Vector2Int(room.Width, z)))
                CreateWall(root.transform, origin + new Vector3(room.Width, room.WallHeight / 2f, z + 0.5f), new Vector3(room.WallThickness, room.WallHeight, 1), wallMat);
        }

        return root;
    }

    private static void CreateWall(Transform parent, Vector3 pos, Vector3 scale, Material mat)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.transform.parent = parent;
        wall.transform.position = pos;
        wall.transform.localScale = scale;
        wall.GetComponent<Renderer>().material = mat;
    }

    //private static void CreateDoorBetween(Room a, Room b)
    //{
    //    Vector2Int doorPos;

    //    if (a.Position.x == b.Position.x) // Vertical neighbor
    //    {
    //        int x = a.Position.x;
    //        int y = Mathf.Max(a.Position.y, b.Position.y);
    //        doorPos = new Vector2Int(x + a.Width / 2, y);
    //    }
    //    else // Horizontal neighbor
    //    {
    //        int y = a.Position.y;
    //        int x = Mathf.Max(a.Position.x, b.Position.x);
    //        doorPos = new Vector2Int(x, y + a.Height / 2);
    //    }

    //    // You could destroy a wall at that position, or skip placing it there.
    //    // Since our walls are cubes, you'll want to mark that position as a "door opening"
    //    // and skip generating a wall at that part.
    //}
}
