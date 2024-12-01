using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyController : MonoBehaviour
{
    public Tilemap mapTilemap;       // Reference to the map tilemap
    public TwoDRPGtilemap mapScript; // Reference to the TwoDRPGtilemap script
    public Vector3Int enemyPosition; // Enemy's position in tile coordinates
    public Transform player;         // Reference to the player object
    private bool isEnemyTurn = false; // Determines if it's the enemy's turn
    private const float enemyZPosition = -1f;


    private void Start()
    {
        if(mapScript == null || mapTilemap == null || player == null)
        {
            return;
        }

        enemyPosition = FindValidSpawnPosition();
        Vector3 worldPosition = mapTilemap.CellToWorld(enemyPosition);
        worldPosition.z = enemyZPosition;
        transform.position = worldPosition;
    }

    private void Update()
    {
        if (isEnemyTurn)
        {
            MoveTowardPlayer();
            isEnemyTurn = false;  //End the enemy's turn
        }
    }

    public void TriggerEnemyTurn()
    {
        isEnemyTurn = true;
    }

    private void MoveTowardPlayer()
    {
        Vector3Int playerTilePosition = mapTilemap.WorldToCell(player.transform.position);
        Vector3Int moveDirection = Vector3Int.zero;

        if (enemyPosition.x < playerTilePosition.x) moveDirection = Vector3Int.right;
        else if (enemyPosition.x > playerTilePosition.x) moveDirection = Vector3Int.left;
        else if (enemyPosition.y < playerTilePosition.y) moveDirection = Vector3Int.up;
        else if (enemyPosition.y > playerTilePosition.y) moveDirection = Vector3Int.down;

        Vector3Int newPosition = enemyPosition + moveDirection;

        if (IsValidPosition(newPosition))
        {
            enemyPosition = newPosition;
            Vector3 worldPosition = mapTilemap.CellToWorld(enemyPosition);
            worldPosition.z = enemyZPosition;
            transform.position = worldPosition;
        }
    }

    private Vector3Int FindValidSpawnPosition()
    {
        Vector3Int spawnPosition = Vector3Int.zero;
        int mapWidth = mapTilemap.size.x;    //get width of map
        int mapHeight = mapTilemap.size.y;   //get height of map

        bool foundValidPosition = false;   //flag to track when valid pos if found
        while (!foundValidPosition)   //loop until a valid pos is found
        {
            spawnPosition = new Vector3Int(Random.Range(1, mapWidth - 1), Random.Range(1, mapHeight - 1), 0);    //randomly generate a spawn position within bounds of map

            TileBase tile = mapTilemap.GetTile(spawnPosition);   //get the tile at spawn position
            if (tile != mapScript.wallTile && tile != mapScript.chestTile)   //check if tile is now a wall or chest (invalid tiles)
            {
                foundValidPosition = true;   //found pos valid, exit loop
            }
        }
        return spawnPosition;   //return valid spawn pos
    }

    private bool IsValidPosition(Vector3Int position)
    {
        if (position.x < 0 || position.x >= mapTilemap.size.x || position.y < 0 || position.y >= mapTilemap.size.y)
        {
            return false;
        }

        TileBase tile = mapTilemap.GetTile(position);
        return tile != mapScript.wallTile && tile != mapScript.chestTile;
    }
}
