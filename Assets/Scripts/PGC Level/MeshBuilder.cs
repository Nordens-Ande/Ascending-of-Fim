using System.Collections.Generic;
using UnityEngine;

public static class MeshBuilder
{
    //Dörrarna skall kunna befinnas på float värden, dess position skall vara mittpunkten av dörren och deras storlek bör kunnas förändras (inom float värden)
    //Fixa också så att vi skapar endast en vägg eller fler beroende på antal dörringångar (1 dörr två väggar, 2 dörrar 3 väggar osv)

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
            //Front vägg(z = 0)
            //if (!room.Doorways.Contains(new Vector2Int(x, 0)))
            //    CreateWall(root.transform, origin + new Vector3(x + 0.5f, room.WallHeight / 2f, 0), new Vector3(1, room.WallHeight, room.WallThickness), wallMat);

            //Back vägg (z = height)
            //if (!room.Doorways.Contains(new Vector2Int(x, room.Height)))
            //    CreateWall(root.transform, origin + new Vector3(x + 0.5f, room.WallHeight / 2f, room.Height), new Vector3(1, room.WallHeight, room.WallThickness), wallMat);
        }

        //for (int z = 0; z < room.Height; z++)
        //{
        //    //Vänster vägg (x = 0)
        //    if (!room.Doorways.Contains(new Vector2Int(0, z)))
        //        CreateWall(root.transform, origin + new Vector3(0, room.WallHeight / 2f, z + 0.5f), new Vector3(room.WallThickness, room.WallHeight, 1), wallMat);

        //    //Höger vägg (x = width)
        //    if (!room.Doorways.Contains(new Vector2Int(room.Width, z)))
        //        CreateWall(root.transform, origin + new Vector3(room.Width, room.WallHeight / 2f, z + 0.5f), new Vector3(room.WallThickness, room.WallHeight, 1), wallMat);
        //}


        //BoundsInt roomBounds = room.GetBounds();
        //float doorSize = room.DoorSize;

        //List<float> frontDoors = room.GetDoorways(Vector2.down);
        //if (frontDoors.Count > 0)
        //{
        //    float previousX = 0;
        //    float doorX = 0;
        //    for (int i = 0; i < frontDoors.Count; i++)
        //    {
        //        doorX = frontDoors[i];
        //        CreateWall(root.transform, origin + new Vector3(previousX + (doorX - previousX - doorSize / 2f) / 2f, room.WallHeight / 2f, 0), new Vector3(doorX - previousX - doorSize / 2f, room.WallHeight, room.WallThickness), wallMat);
        //        //Debug.Log("Position: " + origin + new Vector3(previousX + (doorX - previousX - doorSize / 2f) / 2f, room.WallHeight / 2f, 0));
        //        //Debug.Log("Position: " + (origin + new Vector3(previousX + (doorX - previousX - doorSize / 2f) / 2f, room.WallHeight / 2f, 0)));
        //        //Debug.Log("PreviousX: " + previousX);
        //        //Debug.Log("DoorX: " + doorX);
        //        //Debug.Log("DoorSize: " + doorSize / 2f);
        //        previousX = doorX + doorSize / 2f;
        //    }
        //    CreateWall(root.transform, origin + new Vector3(previousX + (room.Width - previousX) / 2f, room.WallHeight / 2f, 0), new Vector3(room.Width - previousX, room.WallHeight, room.WallThickness), wallMat);
        //}
        //else
        //{
        //    CreateWall(root.transform, origin + new Vector3(room.Width / 2f, room.WallHeight / 2f, 0), wallScaleZ, wallMat);
        //}

        //Debug.Log(frontDoors.Count);
        //Debug.Log(origin);

        CreateWallsWithDoorways(root.transform, origin, room.GetBounds(), room, wallMat);

        return root;
    }

    private static void CreateWallsWithDoorways(Transform parent, Vector3 origin, BoundsInt roomBounds, Room room, Material wallMat)
    {
        Dictionary<Vector2, Vector3> directionOffsets = new()
        {
            { Vector2.down, Vector3.forward },  // Front wall (Z+)
            { Vector2.up, Vector3.back },       // Back wall (Z-)
            { Vector2.left, Vector3.right },    // Left wall (X-)
            { Vector2.right, Vector3.left }     // Right wall (X+)
        };

        foreach (var dir in directionOffsets.Keys)
        {
            List<float> doorPositions = room.GetDoorways(dir);
            Vector3 wallNormal = directionOffsets[dir];
            bool horizontal = (dir == Vector2.down || dir == Vector2.up);
            float roomLength = horizontal ? room.Width : room.Height;
            float wallThickness = room.WallThickness;
            float doorSize = room.DoorSize;

            // Wall position and direction
            Vector3 baseOffset = Vector3.zero;
            Vector3 wallSize;
            if (dir == Vector2.down)
                baseOffset = new Vector3(0, 0, 0); // Front wall
            else if (dir == Vector2.up)
                baseOffset = new Vector3(0, 0, room.Height); // Back wall
            else if (dir == Vector2.left)
                baseOffset = new Vector3(0, 0, 0); // Left wall
            else if (dir == Vector2.right)
                baseOffset = new Vector3(room.Width, 0, 0); // Right wall

            Vector3 wallDir = (horizontal ? Vector3.right : Vector3.forward);
            Vector3 offsetDir = (horizontal ? Vector3.right : Vector3.forward);

            Debug.Log("Amount doors: " + doorPositions.Count + " with dir: " + dir);

            if (doorPositions.Count > 0)
            {
                float previousPos = 0;
                for (int i = 0; i < doorPositions.Count; i++)
                {
                    float doorPos = doorPositions[i];
                    float wallSegmentLength = doorPos - previousPos - doorSize / 2f;

                    if (wallSegmentLength > 0)
                    {
                        Vector3 segmentCenter = origin + baseOffset + offsetDir * (previousPos + wallSegmentLength / 2f);
                        wallSize = horizontal
                            ? new Vector3(wallSegmentLength, room.WallHeight, wallThickness)
                            : new Vector3(wallThickness, room.WallHeight, wallSegmentLength);

                        CreateWall(parent, segmentCenter + new Vector3(0, room.WallHeight / 2f, 0), wallSize, wallMat);
                    }

                    previousPos = doorPos + doorSize / 2f;
                }

                float lastSegmentLength = roomLength - previousPos;
                if (lastSegmentLength > 0)
                {
                    Vector3 segmentCenter = origin + baseOffset + offsetDir * (previousPos + lastSegmentLength / 2f);
                    wallSize = horizontal
                        ? new Vector3(lastSegmentLength, room.WallHeight, wallThickness)
                        : new Vector3(wallThickness, room.WallHeight, lastSegmentLength);

                    CreateWall(parent, segmentCenter + new Vector3(0, room.WallHeight / 2f, 0), wallSize, wallMat);
                }
            }
            else
            {
                // Single full wall
                Vector3 center = origin + baseOffset + offsetDir * (roomLength / 2f);
                wallSize = horizontal
                    ? new Vector3(roomLength, room.WallHeight, wallThickness)
                    : new Vector3(wallThickness, room.WallHeight, roomLength);

                CreateWall(parent, center + new Vector3(0, room.WallHeight / 2f, 0), wallSize, wallMat);
            }
        }
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
