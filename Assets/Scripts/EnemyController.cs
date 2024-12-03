using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyController : MonoBehaviour
{
    public Vector3Int enemyPosition; // Enemy's position in tile coordinates
    public Tilemap mapTilemap;       // Reference to the map tilemap
    private TwoDRPGtilemap mapScript; // Reference to the TwoDRPGtilemap script
    public Transform player;         // Reference to the player object
    private const float enemyZPosition = -1f;
    private bool isTurnActive = false; // Determines if it's the enemy's turn


    void Start()
    {
        mapScript = FindObjectOfType<TwoDRPGtilemap>();
        if (mapScript == null)  //check if it was found, otherwise log as error
        {
            Debug.LogError("TwoDRPGtilemap not found in scene.");
            return;
        }

        enemyPosition = FindValidSpawnPosition();
        Vector3 worldPosition = mapTilemap.CellToWorld(enemyPosition);
        worldPosition.z = enemyZPosition;
        transform.position = worldPosition;
    }

    public void StartTurn()
    {
        isTurnActive = true;      
        StartCoroutine(EnemyTurnCoroutine());      
    }

    private IEnumerator EnemyTurnCoroutine()
    {     
        yield return new WaitForSeconds(0.5f);
        MoveTowardPlayer();
        isTurnActive = false;
        FindObjectOfType<TurnManager>().EndEnemyTurn();
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
            if (tile != mapScript.wallTile && tile != mapScript.chestTile)
            {
                foundValidPosition = true;
            }
        }
        return spawnPosition;
    }

    bool IsValidPosition(Vector3Int position)
    {
        if (position.x < 0 || position.x >= mapTilemap.size.x || position.y < 0 || position.y >= mapTilemap.size.y)
        {
            return false;
        }

        TileBase tile = mapTilemap.GetTile(position);
        if (tile == mapScript.wallTile || tile == mapScript.chestTile)   //check if tile is a wall or chest tile
        {
            return false;  //pos is invalid
        }

        return true;  //pos is valid
    }
}
