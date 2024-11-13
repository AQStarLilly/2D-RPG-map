using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;
using System.IO.MemoryMappedFiles;

public class TwoDRPGtilemap : MonoBehaviour
{
    public Tilemap tilemap;
    public Tile wallTile, floorTile, doorTile, chestTile;

    private char[,] mapArray;

    private void Start()
    {
        string mapData = LoadPremadeMap();

        if (string.IsNullOrEmpty(mapData))  //check if the pre-made map is empty or null
        {
            Debug.LogWarning("Pre-made map is empty or could not be loaded. Generating a random map instead.");

            mapData = GenerateMapString(15, 10);  //fallback for generating a new map if pre-made one is missing - try to add any size capabilities to an extent if possible, although not needed
        }
        else
        {
            Debug.Log("Loaded pre-made map data:\n" + mapData);
        }

        ConvertMapToTilemap(mapData);    
    }

    public string LoadPremadeMap() 
    {
        TextAsset[] mapFiles = Resources.LoadAll<TextAsset>("");
        
        foreach(TextAsset mapFile in mapFiles)
        {
            string mapData = mapFile.text;
            string[] rows = mapData.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            if(rows.Length >=10 &&  rows[0].Trim().Length >= 15)
            {
                Debug.Log("Found valid map file: " + mapFile.name);
                return mapData;
            }
        }
        Debug.LogWarning("No valid map files found with the minimum dimensions of 15x10.");
        return null;       
    }


    public void ConvertMapToTilemap(String mapData)
    {
        if (mapData == null)
        {
            Debug.LogError("Map data is empty or could not be loaded.");
            return;
        }


        tilemap.ClearAllTiles();
        string[] rows = mapData.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        int height = rows.Length;
        int width = rows[0].Trim().Length;  //Ensure consistent width by using the first row's trimmed length

        for(int y = 0; y < height; y++)
        {
            string row = rows[y].Trim();   //trim each row to remove any excess whitespace

            if (row.Length != width)   //Ensure each row has correct width to avoid indexing errors
            {
                Debug.LogError($"Row {y} has an inconsistent length. Expected {width}, got {row.Length}.");
                continue;
            }

            for(int x = 0; x < width; x++)
            {
                char tileChar = row[x];
                Vector3Int position = new Vector3Int(x, y, 0);

                switch (tileChar)
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

    public string GenerateMapString(int width, int height)
    {
        System.Text.StringBuilder mapString = new System.Text.StringBuilder();
        mapArray = new char[height, width];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                //place walls for border
                if (y == 0 || y == height - 1 || x == 0 || x == width - 1)
                {
                    mapString.Append('#');
                    mapArray[y, x] = '#';
                }
                else
                {
                    char tileChar = ' ';
                    if (UnityEngine.Random.Range(0, 10) < 2)
                    {
                        tileChar = '$';  //random chance for a chest
                    }
                    else if (UnityEngine.Random.Range(0, 10) < 2)
                    {
                        tileChar = 'O';  //random chance for a door
                    }

                    mapString.Append(tileChar);
                    mapArray[y, x] = tileChar;
                }
            }
            mapString.AppendLine();
        }
        return mapString.ToString();
    }
}
