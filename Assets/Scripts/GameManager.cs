using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    public Dictionary<Vector3Int, GameObject> mapObjects = new Dictionary<Vector3Int, GameObject>();

    [Header("UI Panels")]
    public GameObject gameOverPanel;
    public GameObject gameWonPanel;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializeMapObjects();
    }

    private void InitializeMapObjects()
    {
        PlayerController[] players = FindObjectsOfType<PlayerController>();
        foreach (PlayerController player in players)
        {
            mapObjects[player.playerPosition] = player.gameObject;
        }

        EnemyController[] enemies = FindObjectsOfType<EnemyController>();
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

    public void RemoveObjectFromMap(GameObject obj)
    {
        foreach (var kvp in mapObjects)
        {
            if(kvp.Value == obj)
            {
                mapObjects.Remove(kvp.Key);
                Debug.Log($"{obj.name} removed from mapObjects.");
                return;
            }
        }
    }

    private void HandleDeath(GameObject obj)
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

    private void ShowGameOverScreen()
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

    private void ShowGameWonScreen()
    {
        Debug.Log("Activating Game Won Panel.");
        gameWonPanel.SetActive(true); // Show Victory screen
        Time.timeScale = 0; // Pause the game
    }
}
