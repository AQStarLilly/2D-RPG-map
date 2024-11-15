using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Player2Controller : MonoBehaviour
{
    public Vector3Int player2Position;  //players position on the map in tile coordinates
    public Tilemap mapTilemap;   //reference to the tilemap where the map is stored
    private TwoDRPGtilemap mapScript;   //reference to the TwoDRPGtilemap script to access tile data
    private const float playerZPosition = -1f;  //makes sure player spawns above map instead of below

    // Start is called before the first frame update
    void Start()
    {
        mapScript = FindObjectOfType<TwoDRPGtilemap>();  //Find the TwoDRPGtilemap script in the scene
        if (mapScript == null)  //check if it was found, otherwise log as error
        {
            Debug.LogError("TwoDRPGtilemap not found in scene.");
            return;
        }

        player2Position = FindValidSpawnPosition();  //find a valid spawn position for the player
        Debug.Log($"Player tile position is {player2Position}");

        Vector3 worldPosition = mapTilemap.CellToWorld(player2Position);  //convert the player position from tile cords to world position
        Debug.Log($"World position before setting Z: {worldPosition}");

        worldPosition.z = transform.position.z;   //set the player's world position with the correct z value
        Debug.Log($"Final world position: {worldPosition}");

        transform.position = worldPosition;   //set the player's transform to the new world position
    }

    // Update is called once per frame
    void Update()
    {
        Vector3Int moveDirection = Vector3Int.zero;  //initialize move direction as zero
        if (Input.GetKeyDown(KeyCode.UpArrow)) moveDirection = Vector3Int.up;
        if (Input.GetKeyDown(KeyCode.DownArrow)) moveDirection = Vector3Int.down;
        if (Input.GetKeyDown(KeyCode.LeftArrow)) moveDirection = Vector3Int.left;
        if (Input.GetKeyDown(KeyCode.RightArrow)) moveDirection = Vector3Int.right;

        if (moveDirection != Vector3Int.zero)  //if a movement direction was input
        {
            Vector3Int newPosition = player2Position + moveDirection;  //calc the new player pos based on the move direction

            if (IsValidPosition(newPosition))  //check if new pos is valid
            {
                player2Position = newPosition;   //update player pos
                Vector3 worldPosition = mapTilemap.CellToWorld(player2Position);   //convert the new player pos to world coordinates
                worldPosition.z = playerZPosition;   //ensure player's z position doesn't change

                transform.position = worldPosition;
            }
        }
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
            if (tile != mapScript.wallTile && tile != mapScript.chestTile)   //check if tile is now a wall or chest (invalid tiles)
            {
                foundValidPosition = true;   //found pos valid, exit loop
            }
        }
        return spawnPosition;   //return valid spawn pos
    }

    bool IsValidPosition(Vector3Int position)   //check if a given position is a valid move (not outside bounds or invalid tile)
    {
        if (position.x < 0 || position.x >= mapTilemap.size.x || position.y < 0 || position.y >= mapTilemap.size.y)  //ensure position is within bounds
        {
            return false;
        }

        TileBase tile = mapTilemap.GetTile(position);  //get tile at given pos
        if (tile == mapScript.wallTile || tile == mapScript.chestTile)   //check if tile is a wall or chest tile
        {
            return false;  //pos is invalid
        }

        return true;  //pos is valid
    }
}