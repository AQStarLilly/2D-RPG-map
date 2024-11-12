using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    public Vector3Int playerPosition;
    public Tilemap mapTilemap;
    private TwoDRPGtilemap mapScript;
    private const float playerZPosition = -1f;

    // Start is called before the first frame update
    void Start()
    {
        mapScript = FindObjectOfType<TwoDRPGtilemap>();
        if(mapScript == null)
        {
            Debug.LogError("TwoDRPGtilemap nop found in scene.");
            return;
        }

        playerPosition = FindValidSpawnPosition();
        Debug.Log($"Player tile position is {playerPosition}");

        Vector3 worldPosition = mapTilemap.CellToWorld(playerPosition);
        Debug.Log($"World position before setting Z: {worldPosition}");

        worldPosition.z = transform.position.z;
        Debug.Log($"Final world position: {worldPosition}");

        transform.position = worldPosition;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3Int moveDirection = Vector3Int.zero;
        if (Input.GetKeyDown(KeyCode.W)) moveDirection = Vector3Int.up;
        if (Input.GetKeyDown(KeyCode.S)) moveDirection = Vector3Int.down;
        if (Input.GetKeyDown(KeyCode.A)) moveDirection = Vector3Int.left;
        if (Input.GetKeyDown(KeyCode.D)) moveDirection = Vector3Int.right;

        if (moveDirection != Vector3Int.zero)
        {
            Vector3Int newPosition = playerPosition + moveDirection;

            if (IsValidPosition(newPosition))
            {
                playerPosition = newPosition;
                Vector3 worldPosition = mapTilemap.CellToWorld(playerPosition);
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
