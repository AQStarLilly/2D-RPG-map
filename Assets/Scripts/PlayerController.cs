using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    public Vector3Int playerPosition;  //players position on the map in tile coordinates
    public Tilemap mapTilemap;   //reference to the tilemap where the map is stored
    private TwoDRPGtilemap mapScript;   //reference to the TwoDRPGtilemap script to access tile data
    private const float playerZPosition = -1f;  //makes sure player spawns above map instead of below
    public bool isPlayer1 = true;  //checkl if player 1 or 2
    public bool isTurnActive = false;
    
    private KeyCode upKey;
    private KeyCode downKey;
    private KeyCode leftKey;
    private KeyCode rightKey;

    // Start is called before the first frame update
    void Start()
    {
        mapScript = FindObjectOfType<TwoDRPGtilemap>();  //Find the TwoDRPGtilemap script in the scene
        if(mapScript == null)  //check if it was found, otherwise log as error
        {
            Debug.LogError("TwoDRPGtilemap not found in scene.");
            return;
        }

        if (isPlayer1)
        {
            upKey = KeyCode.W;
            downKey = KeyCode.S;
            leftKey = KeyCode.A;
            rightKey = KeyCode.D;
        }
        else
        {
            upKey = KeyCode.UpArrow;
            downKey = KeyCode.DownArrow;
            leftKey = KeyCode.LeftArrow;
            rightKey = KeyCode.RightArrow;
        }

        playerPosition = FindValidSpawnPosition();  //find a valid spawn position for the player
        Debug.Log($"Player {(isPlayer1 ? "1" : "2")} tile position is {playerPosition}");

        Vector3 worldPosition = mapTilemap.CellToWorld(playerPosition);  //convert the player position from tile cords to world position
        Debug.Log($"World position before setting Z: {worldPosition}");

        worldPosition.z = transform.position.z;   //set the player's world position with the correct z value
        Debug.Log($"Final world position: {worldPosition}");

        transform.position = worldPosition;   //set the player's transform to the new world position
    }

    public void StartTurn()
    {
        isTurnActive = true;
    }

    private void UpdatePosition(Vector3Int oldPosition, Vector3Int newPosition)
    {
        GameManager.Instance.mapObjects.Remove(oldPosition);
        GameManager.Instance.mapObjects[newPosition] = gameObject;
    }
    // Update is called once per frame
    void Update()
    {
        if (!isTurnActive) return;

        Vector3Int moveDirection = Vector3Int.zero;  //initialize move direction as zero
        if (Input.GetKeyDown(upKey)) moveDirection = Vector3Int.up;
        if (Input.GetKeyDown(downKey)) moveDirection = Vector3Int.down;
        if (Input.GetKeyDown(leftKey)) moveDirection = Vector3Int.left;
        if (Input.GetKeyDown(rightKey)) moveDirection = Vector3Int.right;

        if (moveDirection != Vector3Int.zero)  //if a movement direction was input
        {
            Vector3Int newPosition = playerPosition + moveDirection;  //calc the new player pos based on the move direction           
            Debug.Log($"Player trying to move to {newPosition}");

            if (GameManager.Instance.mapObjects.ContainsKey(newPosition)) // Check if tile is occupied
            {
                Debug.Log($"Tile at {newPosition} is occupied.");
                TryAttack(newPosition); // Attempt to attack
                EndTurn();
            }
            else if (IsValidPosition(newPosition)) // Otherwise move
            {
                Debug.Log($"Moving to valid position {newPosition}");
                MoveToPosition(newPosition);
            }
            else
            {
                Debug.Log($"Invalid position: {newPosition}");
            }
        }
    }

    private void TryAttack(Vector3Int targetPosition)
    {
        Debug.Log($"Player attempting to attack at {targetPosition}");

        if (GameManager.Instance.mapObjects.TryGetValue(targetPosition, out GameObject target))
        {
            Debug.Log($"Target found at {targetPosition}: {target.name}");

            if (target.CompareTag("Enemy"))
            {
                HealthSystem enemyHealth = target.GetComponent<HealthSystem>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(5);
                    Debug.Log("Player attacked the enemy!");
                }
                else
                {
                    Debug.LogWarning("Enemy found but has no HealthSystem!");
                }
            }
            else
            {
                Debug.Log("Target is not an enemy.");
            }
        }
        else
        {
            Debug.Log("No target found at position.");
        }
    }

    private void MoveToPosition(Vector3Int newPosition)
    {
        Vector3Int oldPosition = playerPosition;
        playerPosition = newPosition;

        GameManager.Instance.mapObjects.Remove(oldPosition);
        GameManager.Instance.mapObjects[newPosition] = gameObject;

        Vector3 worldPosition = mapTilemap.CellToWorld(playerPosition);
        worldPosition.z = playerZPosition;
        transform.position = worldPosition;

        EndTurn();
    }

    void EndTurn()
    {
        isTurnActive = false;
        FindObjectOfType<TurnManager>().EndPlayerTurn();
    }

    Vector3Int FindValidSpawnPosition()   //finds a random valid spawn point for player
    {
        Vector3Int spawnPosition = Vector3Int.zero;  
        int mapWidth = mapTilemap.size.x;    //get width of map
        int mapHeight = mapTilemap.size.y;   //get height of map

        bool foundValidPosition = false;   //flag to track when valid pos if found
        while (!foundValidPosition)   //loop until a valid pos is found
        {          
            spawnPosition = new Vector3Int(Random.Range(1, mapWidth - 1), Random.Range(1, mapHeight - 1), 0);    //randomly generate a spawn position within bounds of map

            TileBase tile = mapTilemap.GetTile(spawnPosition);   //get the tile at spawn position
            if(tile != mapScript.wallTile && tile != mapScript.chestTile)   //check if tile is now a wall or chest (invalid tiles)
            {
                foundValidPosition = true;   //found pos valid, exit loop
            }
        }
        return spawnPosition;   //return valid spawn pos
    }

    bool IsValidPosition(Vector3Int position)   //check if a given position is a valid move (not outside bounds or invalid tile)
    {
        if(position.x < 0 || position.x >= mapTilemap.size.x || position.y < 0 || position.y >= mapTilemap.size.y)  //ensure position is within bounds
        {
            return false; 
        }

        TileBase tile = mapTilemap.GetTile(position);  //get tile at given pos
        if(tile == mapScript.wallTile || tile == mapScript.chestTile)   //check if tile is a wall or chest tile
        {
            return false;  //pos is invalid
        }

        return true;  //pos is valid
    }
}
