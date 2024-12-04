using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;    //singleton instance of the gamemanager
    
    public Dictionary<Vector3Int, GameObject> mapObjects = new Dictionary<Vector3Int, GameObject>();  //dictionary to store all objects on the map and their positions

    [Header("UI Panels")]   //UI panels for gameOver and gameWon screens respectively
    public GameObject gameOverPanel;
    public GameObject gameWonPanel;

    private void Awake()
    {
        if(Instance == null)  //ensure only 1 instance of the game manager exists
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);  //destory duplciate gamemanagers
        }
    }

    private void Start()
    {
        InitializeMapObjects();  //Initialize the map objects dictionary with player and enemy position
    }

    private void InitializeMapObjects()  //populate the map objects dictionary with player and enemy position
    {
        PlayerController[] players = FindObjectsOfType<PlayerController>();  //add all players to dictionary
        foreach (PlayerController player in players)
        {
            mapObjects[player.playerPosition] = player.gameObject;
        }

        EnemyController[] enemies = FindObjectsOfType<EnemyController>();  //add all enemies to dictionary
        foreach (EnemyController enemy in enemies)
        {
            mapObjects[enemy.enemyPosition] = enemy.gameObject;
        }
    }

    private void OnEnable()
    {
        HealthSystem.OnDeath += HandleDeath;   
    }

    private void OnDisable()
    {
        HealthSystem.OnDeath -= HandleDeath;
    }

    public void RemoveObjectFromMap(GameObject obj)  //remove an object from the mapObjects dictionary
    {
        foreach (var kvp in mapObjects)
        {
            if(kvp.Value == obj)
            {
                mapObjects.Remove(kvp.Key); //remove the object from the dictionary
                Debug.Log($"{obj.name} removed from mapObjects.");
                return;
            }
        }
    }

    private void HandleDeath(GameObject obj)  //handle death of player or enemy
    {
        if (obj.CompareTag("Player"))
        {
            Debug.Log("Player died. Game Over.");
            ShowGameOverScreen();
        }
        else if (obj.CompareTag("Enemy"))
        {
            Debug.Log("Enemy defeated");
            RemoveObjectFromMap(obj);

            CheckForVictory();
        }
    }

    private void ShowGameOverScreen()    //show gameover screen and pause the game
    {
        Debug.Log("Activating Game Over screen.");
        gameOverPanel.SetActive(true); // Show Game Over screen
        Time.timeScale = 0; // Pause the game
    }

    private void CheckForVictory()  
    {
        // If no enemies remain, show the victory screen
        bool allEnemiesDefeated = true;

        foreach (var obj in mapObjects.Values)
        {
            if (obj.CompareTag("Enemy"))
            {
                Debug.Log($"Enemy still active: {obj.name}");
                allEnemiesDefeated = false;
                break;
            }
        }

        if (allEnemiesDefeated)
        {
            Debug.Log("All enemies defeated. Victory!");
            ShowGameWonScreen();
        }
        else
        {
            Debug.Log("Victory conditions not met yet.");
        }       
    }

    private void ShowGameWonScreen()  //show the game won screen and pause the game
    {
        Debug.Log("Activating Game Won Panel.");
        gameWonPanel.SetActive(true); // Show Victory screen
        Time.timeScale = 0; // Pause the game
    }
}
