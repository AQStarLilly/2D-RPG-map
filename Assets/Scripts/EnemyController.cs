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

        enemyPosition = FindValidSpawnPosition();    //finds valid spawn for enemy
        //convert tile position to world position 
        Vector3 worldPosition = mapTilemap.CellToWorld(enemyPosition);   
        worldPosition.z = enemyZPosition;
        transform.position = worldPosition;

        //register the enemy in the GameManager's mapObjects dictionary
        GameManager.Instance.mapObjects[enemyPosition] = gameObject;
        Debug.Log($"Enemy Registered at {enemyPosition}");
    }

    public void StartTurn()  //activate the enemies turn and start the turn coroutine
    {
        isTurnActive = true;      
        StartCoroutine(EnemyTurnCoroutine());      
    }

    private IEnumerator EnemyTurnCoroutine()
    {     
        yield return new WaitForSeconds(0.5f);  //simulate turn-based movement
        MoveTowardPlayer();   
        isTurnActive = false;   //end the enemies turn
        FindObjectOfType<TurnManager>().EndEnemyTurn();   //notify the turn manager that enemies turn ended
    }

    private void MoveTowardPlayer()
    {
        Vector3Int playerTilePosition = mapTilemap.WorldToCell(player.transform.position);  //get player position in tile coords
        //determine the direction to move towards the player
        Vector3Int moveDirection = Vector3Int.zero;
        if (enemyPosition.x < playerTilePosition.x) moveDirection = Vector3Int.right;
        else if (enemyPosition.x > playerTilePosition.x) moveDirection = Vector3Int.left;
        else if (enemyPosition.y < playerTilePosition.y) moveDirection = Vector3Int.up;
        else if (enemyPosition.y > playerTilePosition.y) moveDirection = Vector3Int.down;

        Vector3Int newPosition = enemyPosition + moveDirection;   //calculate new position based on move direction

        if (GameManager.Instance.mapObjects.ContainsKey(newPosition))  //check if new position is occupied
        {
            GameObject target = GameManager.Instance.mapObjects[newPosition]; //if the new position contains player, attack them
            if (target.CompareTag("Player"))
            {
                HealthSystem playerHealth = target.GetComponent<HealthSystem>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(5);
                    Debug.Log("Enemy attacked the player!");
                }
            }
        }
        else if (IsValidPosition(newPosition))  //if the new position is valid and unoccupied, move the enemy
        {
            UpdatePosition(enemyPosition, newPosition);  //update the enemy position in the game manager

            enemyPosition = newPosition;
            Vector3 worldPosition = mapTilemap.CellToWorld(enemyPosition);
            worldPosition.z = enemyZPosition;
            transform.position = worldPosition;
        }
    }

    private void UpdatePosition(Vector3Int oldPosition, Vector3Int newPosition)
    {
        GameManager.Instance.mapObjects.Remove(oldPosition);      //remove old pos from GameManager's mapObjects dictionary
        GameManager.Instance.mapObjects[newPosition] = gameObject;  //add new pos to GameManager's mapObjects dictionary
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
