using System.Collections.Generic;
using UnityEngine;

public interface ITileable
{
    public List<Vector2Int> GetOccupiedTiles();
    //public List<Vector2Int> GetOccupiedTiles(Vector2Int tempPos);
}

public static class MeshBuilder
{
    //D?rrarna skall kunna befinnas p? float v?rden, dess position skall vara mittpunkten av d?rren och deras storlek b?r kunnas f?r?ndras (inom float v?rden)
    //Fixa ocks? s? att vi skapar endast en v?gg eller fler beroende p? antal d?rring?ngar (1 d?rr tv? v?ggar, 2 d?rrar 3 v?ggar osv)

    public static GameObject CreateRoomMesh(Room room, Material wallMat, Material floorMat)
    {
        //Skapar GameObject-et
        GameObject root = new GameObject($"Room_{room.Position}");

        Vector3 origin = new Vector3(room.Position.x, 0, room.Position.y);
        Vector3 size = new Vector3(room.Width, 0, room.Height);

        //Definerar golv
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.isStatic = true;
        floor.transform.parent = root.transform;
        floor.transform.position = origin + size / 2f;
        floor.transform.localScale = new Vector3(size.x, 0.1f, size.z);
        floor.GetComponent<Renderer>().material = floorMat;

        //Definar fyra v?gar (4 directions)
        //Vector3 wallScaleX = new Vector3(room.WallThickness, room.WallHeight, room.Height);
        //Vector3 wallScaleZ = new Vector3(room.Width, room.WallHeight, room.WallThickness);

        CreateWallsWithDoorways(root.transform, origin, room.GetBounds(), room, wallMat);

        return root;
    }
    public static void DecorateRoomMesh(Transform root, Room room)
    {
        foreach (Furniture furniture in room.FurnitureList)
        {
            Debug.Log(furniture.transform.position);
            CreateFurniture(root.transform.root, furniture.gameObject);
        }       
    }

    private static void CreateWallsWithDoorways(Transform parent, Vector3 origin, BoundsInt roomBounds, Room room, Material wallMat)
    {
        Dictionary<Vector2, Vector3> directionOffsets = new()
        {
            { Vector2.down, Vector3.forward },  //Front wall (Z+)
            { Vector2.up, Vector3.back },       //Back wall (Z-)
            { Vector2.left, Vector3.right },    //Left wall (X-)
            { Vector2.right, Vector3.left }     //Right wall (X+)
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
                baseOffset = new Vector3(0, 0, 0); //Front wall
            else if (dir == Vector2.up)
                baseOffset = new Vector3(0, 0, room.Height); //Back wall
            else if (dir == Vector2.left)
                baseOffset = new Vector3(0, 0, 0); //Left wall
            else if (dir == Vector2.right)
                baseOffset = new Vector3(room.Width, 0, 0); //Right wall

            Vector3 wallDir = (horizontal ? Vector3.right : Vector3.forward);
            Vector3 offsetDir = (horizontal ? Vector3.right : Vector3.forward);

            //Debug.Log("Amount doors: " + doorPositions.Count + " with dir: " + dir);

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
                //Single full wall
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
        wall.name = $"{parent.gameObject.name} Wall{new Vector2(pos.x, pos.z)}";
        wall.transform.parent = parent;
        wall.transform.position = pos;
        wall.transform.localScale = scale;
        wall.GetComponent<Renderer>().material = mat;
        wall.layer = 8;
    }

    public static void CreateFurniture(Transform parent, GameObject prefab)
    {
        GameObject furnitureObject = GameObject.Instantiate(prefab);
        furnitureObject.transform.parent = parent;
        furnitureObject.transform.position = prefab.transform.position;
        furnitureObject.name = "Furniture " + furnitureObject.transform.position;
    }
}
