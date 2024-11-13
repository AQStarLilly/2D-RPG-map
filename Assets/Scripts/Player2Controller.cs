using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Player2Controller : MonoBehaviour
{
    public Vector3Int player2Position;
    public Tilemap mapTilemap;
    private TwoDRPGtilemap mapScript;
    private const float playerZPosition = -1f;

    // Start is called before the first frame update
    void Start()   //player 2 spawning outside the map or in walls occassionally
    {
        mapScript = FindObjectOfType<TwoDRPGtilemap>();
        if(mapScript == null)
        {
            Debug.LogError("TwoDRPGtilemap not found in scene.");
            return;
        }

        player2Position = FindValidSpawnPosition();
        Debug.Log($"Player 2 tile position is {player2Position}");

        Vector3 worldPosition = mapTilemap.CellToWorld(player2Position);
        Debug.Log($"World position before setting Z: {worldPosition}");

        worldPosition.z = transform.position.z;
        Debug.Log($"Final world position: {worldPosition}");

        transform.position = worldPosition;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3Int moveDirection = Vector3Int.zero;
        if (Input.GetKeyDown(KeyCode.UpArrow)) moveDirection = Vector3Int.up;
        if (Input.GetKeyDown(KeyCode.DownArrow)) moveDirection = Vector3Int.down;
        if (Input.GetKeyDown(KeyCode.LeftArrow)) moveDirection = Vector3Int.left;
        if (Input.GetKeyDown(KeyCode.RightArrow)) moveDirection = Vector3Int.right;

        if (moveDirection != Vector3Int.zero)
        {
            Vector3Int newPosition = player2Position + moveDirection;

            if (IsValidPosition(newPosition))
            {
                player2Position = newPosition;
                Vector3 worldPosition = mapTilemap.CellToWorld(player2Position);
                worldPosition.z = playerZPosition;

                transform.position = worldPosition;
            }
        }
    }

    Vector3Int FindValidSpawnPosition()
    {
        Vector3Int spawnPosition = Vector3Int.zero;
        int mapWidth = mapTilemap.size.x;
        int mapHeight = mapTilemap.size.y;

        bool foundValidPosition = false;
        while (!foundValidPosition)
        {
            spawnPosition = new Vector3Int(Random.Range(1, mapWidth - 1), Random.Range(1, mapHeight - 1), 0);

            TileBase tile = mapTilemap.GetTile(spawnPosition);
            if(tile != mapScript.wallTile && tile != mapScript.chestTile)
            {
                foundValidPosition = true;
            }
        }
        return spawnPosition;
    }

    bool IsValidPosition(Vector3Int position)
    {
        if(position.x < 0 || position.x >= mapTilemap.size.x || position.y < 0 || position.y >= mapTilemap.size.y)
        {
            return false;
        }

        TileBase tile = mapTilemap.GetTile(position);
        if(tile == mapScript.wallTile || tile == mapScript.chestTile)
        {
            return false;
        }

        return true;
    }
}