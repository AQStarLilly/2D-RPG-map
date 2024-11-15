using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Collections.Generic;

public class TwoDRPGtilemap : MonoBehaviour
{
    public Tilemap tilemap;
    public Tile wallTile, floorTile, doorTile, chestTile;   //tile references for diff map elements

    private char[,] mapArray; //2D array to rep the map in character form (for loading and generating maps)

    private void Start()
    {
        string mapData = LoadPremadeMap();   //attempt to load a premade map

        if (string.IsNullOrEmpty(mapData))  //check if the pre-made map is empty or null
        {
            Debug.LogWarning("Pre-made map is empty or could not be loaded. Generating a random map instead.");

            mapData = GenerateMapString(15, 10);  //fallback for generating a new map if pre-made one is missing
        }
        else
        {
            Debug.Log("Loaded pre-made map data:\n" + mapData);
        }

         ConvertMapToTilemap(mapData);  //convert map data(string) into tilemap tiles
    }

    public string LoadPremadeMap()   //load premade map from resources file
    {
        TextAsset[] mapFiles = Resources.LoadAll<TextAsset>("");  //load all textAsset files from resources folder

        foreach (TextAsset mapFile in mapFiles)  //iterate through each map file and check if it meets the required dimensions
        {
            string mapData = mapFile.text;
            string[] rows = mapData.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            if (rows.Length >= 10 && rows[0].Trim().Length >= 15)  //check for minimum dimensions
            {
                Debug.Log("Found valid map file: " + mapFile.name);
                return mapData;  //return valid map data
            }
        }
        Debug.LogWarning("No valid map files found with the minimum dimensions of 15x10.");
        return null;   //no valid map found, log warning and return null
    }


    public void ConvertMapToTilemap(String mapData)   //convert the loaded map data(string) into the tilemap
    {
        if (string.IsNullOrEmpty(mapData))  //ensure that map data isn't empty
        {
            Debug.LogError("Map data is empty or could not be loaded.");
            return;
        }

        tilemap.ClearAllTiles();    //clear any existing tiles in the tilemap
        string[] rows = mapData.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        int height = rows.Length;               //get the height
        int width = rows[0].Trim().Length;      //Ensure consistent width by using the first row's trimmed length

        for (int y = 0; y < height; y++)  //loop through each row and column to place tiles
        {
            string row = rows[y].Trim();   //trim each row to remove any excess whitespace

            if (row.Length != width)   //Ensure each row has correct width to avoid indexing errors
            {
                Debug.LogError($"Row {y} has an inconsistent length. Expected {width}, got {row.Length}.");
                continue;
            }

            for (int x = 0; x < width; x++)  //loop through each character in the row(each column)
            {
                char tileChar = row[x];    //get the character representing the tile at this position
                Vector3Int position = new Vector3Int(x, y, 0);   //convert to tilemaps position format

                switch (tileChar)   //make the corresponding tile based on the character
                {
                    case '#':
                        tilemap.SetTile(position, wallTile);
                        break;
                    case 'O':
                        tilemap.SetTile(position, doorTile);
                        break;
                    case '$':
                        tilemap.SetTile(position, chestTile);
                        break;
                    default:
                        tilemap.SetTile(position, floorTile);
                        break;
                }
            }
        }
        Debug.Log("Map rendered successfully.");
    }

    public string GenerateMapString(int width, int height)  //generate a random map string with given width and height
    {
        System.Text.StringBuilder mapString = new System.Text.StringBuilder();    //string builder for map construction
        mapArray = new char[height, width];  //2D array to represent the map

        //create wall borders
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                //place walls for border
                if (y == 0 || y == height - 1 || x == 0 || x == width - 1)
                {
                    mapArray[y, x] = '#'; //border wall
                }
                else
                {
                    mapArray[y, x] = ' '; //empty floor by default
                }
                mapString.Append(mapArray[y, x]);   //append the character to the map string
            }
            mapString.AppendLine();   //add newline after each row
        }

        List<Vector2Int> possibleDoorPositions = new List<Vector2Int>();   //list all door positions (along the walls)

        for (int y = 1; y < height - 1; y++)   //add possible door positions on the left and right walls (except corners)
        {
            possibleDoorPositions.Add(new Vector2Int(0, y));            //Left wall
            possibleDoorPositions.Add(new Vector2Int(width - 1, y));    //Right wall
        }

        for (int x = 1; x < width - 1; x++)   //add possible door positions on the top and bottom walls (except corners)
        {
            possibleDoorPositions.Add(new Vector2Int(x, 0));            //Top wall
            possibleDoorPositions.Add(new Vector2Int(x, height - 1));   //Bottom wall
        }

        int doorCount = UnityEngine.Random.Range(1, 5);  //min 1, max 4 doors
        for (int i = 0; i < doorCount && possibleDoorPositions.Count > 0; i++)
        {
            int index = UnityEngine.Random.Range(0, possibleDoorPositions.Count);  //pick a random position
            Vector2Int doorPosition = possibleDoorPositions[index];
            possibleDoorPositions.RemoveAt(index);   //remove chosen position to avoid duplicates

            mapArray[doorPosition.y, doorPosition.x] = 'O';  //place doors
        }

        //Place chests in the corners if available
        List<Vector2Int> cornerPositions = new List<Vector2Int>
        {
            new Vector2Int(1, 1),                        //Top-left corner
            new Vector2Int(1, height - 2),               //bottom-left corner
            new Vector2Int(width - 2, 1),                //top-right corner
            new Vector2Int(width - 2, height - 2)        //bottom-right corner
        };

        foreach (Vector2Int corner in cornerPositions)   //randomly place chess in corner positions
        {
            if (UnityEngine.Random.Range(0, 2) == 0)  //50% chance of placing a chest in each corner
            {
                if (corner.y >= 0 && corner.y < height && corner.x >= 0 && corner.x < width)
                {
                    mapArray[corner.y, corner.x] = '$';   //place chest
                    Debug.Log($"Placed chest at corner ({corner.x}, {corner.y})");
                }
                else
                {
                    Debug.LogWarning($"Corner position {corner} is out of bounds for map size {width}x{height}");
                }
            }
        }

        //Build the map string
        mapString.Clear();
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                mapString.Append(mapArray[y, x]);
            }
            mapString.AppendLine();
        }

        //log and return the generated map string
        Debug.Log("Generated Map:\n" + mapString.ToString());

        return mapString.ToString();
    }
}
    
