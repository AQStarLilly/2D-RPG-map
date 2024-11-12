using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    public Vector3Int playerPosition;
    public Tilemap mapTilemap;
    private TwoDRPGtilemap mapScript;

    // Start is called before the first frame update
    void Start()
    {
        mapScript = FindObjectOfType<TwoDRPGtilemap>();
        if(mapScript == null)
        {
            Debug.LogError("TwoDRPGtilemap nop found in scene.");
            return;
        }


        playerPosition = mapTilemap.WorldToCell(transform.position);
        MovePlayer(Vector3Int.zero);   //initialize position on tilemap
    }

    // Update is called once per frame
    void Update()
    {
        Vector3Int moveDirection = Vector3Int.zero;
        if (Input.GetKeyDown(KeyCode.W)) moveDirection = Vector3Int.up;
        if (Input.GetKeyDown(KeyCode.S)) moveDirection = Vector3Int.down;
        if (Input.GetKeyDown(KeyCode.A)) moveDirection = Vector3Int.left;
        if (Input.GetKeyDown(KeyCode.D)) moveDirection = Vector3Int.right;

        if(moveDirection != Vector3Int.zero)
        {
            Vector3Int newPosition = playerPosition + moveDirection;
            TileBase tile = mapTilemap.GetTile(newPosition);

            if (tile != mapScript.wallTile && tile != mapScript.chestTile)
            {
                MovePlayer(moveDirection);
            }
        }
    }

    void MovePlayer(Vector3Int moveDirection)
    {
        playerPosition += moveDirection;
        transform.position = mapTilemap.CellToWorld(playerPosition);
    }
}
