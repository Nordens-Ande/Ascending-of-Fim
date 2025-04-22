using System.Collections.Generic;
using UnityEngine;

public class TileDebugger : MonoBehaviour
{
    [SerializeField] bool draw = false;
    [SerializeField] Color roomColor = Color.green;
    [SerializeField] Color doorColor = Color.red;
    [SerializeField] Color furnitureColor = Color.yellow;
    [SerializeField] Color checkedColor = Color.blue;

    public HashSet<Vector2Int> roomTiles = new HashSet<Vector2Int>();
    public HashSet<Vector2Int> doorTiles = new HashSet<Vector2Int>();
    public HashSet<Vector2Int> furnitureTiles = new HashSet<Vector2Int>();
    public HashSet<Vector2Int> checkedTiles = new HashSet<Vector2Int>();
    
    private Vector3 tileSize = new Vector3(1, 0.1f, 1); // Flat cube on the XZ plane

    public void ClearTiles()
    {
        roomTiles.Clear();
        doorTiles.Clear();
        furnitureTiles.Clear();
        checkedTiles.Clear();
    }

    private void OnDrawGizmos()
    {
        if (!draw)
            return;

        DrawTileSet(roomTiles, roomColor);
        DrawTileSet(doorTiles, doorColor);
        DrawTileSet(furnitureTiles, furnitureColor);
        DrawTileSet(checkedTiles, checkedColor);

        //Gizmos.color = roomColor;
        //foreach (Vector2Int tile in roomTiles)
        //{
        //    // Convert Vector2Int to Vector3 (optional: adjust for grid origin/offset)
        //    Vector3 worldPos = new Vector3(tile.x, 0, tile.y);
        //    Gizmos.DrawCube(worldPos + tileSize * 0.5f, tileSize);
        //}

        //Gizmos.color = doorColor;
        //foreach (Vector2Int tile in doorTiles)
        //{
        //    // Convert Vector2Int to Vector3 (optional: adjust for grid origin/offset)
        //    Vector3 worldPos = new Vector3(tile.x, 0, tile.y);
        //    Gizmos.DrawCube(worldPos + tileSize * 0.5f, tileSize);
        //}

        //Gizmos.color = furnitureColor;
        //foreach (Vector2Int tile in furnitureTiles)
        //{
        //    // Convert Vector2Int to Vector3 (optional: adjust for grid origin/offset)
        //    Vector3 worldPos = new Vector3(tile.x, 0, tile.y);
        //    Gizmos.DrawCube(worldPos + tileSize * 0.5f, tileSize);
        //}
    }

    void DrawTileSet(HashSet<Vector2Int> tiles, Color color)
    {
        Gizmos.color = color;
        foreach (Vector2Int tile in tiles)
        {
            Vector3 worldPos = new Vector3(tile.x, 0, tile.y);
            Gizmos.DrawCube(worldPos + tileSize * 0.5f, tileSize);
        }
    }
}

