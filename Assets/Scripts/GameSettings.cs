using System;
using System.IO;
using UnityEngine;

[Serializable] 
public class GameSettings
{
    public int maxPlayerHealth;
    public int playerDamage;
    public int maxEnemyHealth;
    public int enemyDamage;

    private static string settingsFilePath = Path.Combine(Application.dataPath, "../game_settings.json");

    public static GameSettings LoadSettings()
    {
        try
        {
            if (!File.Exists(settingsFilePath))
            {
                Debug.LogWarning($"Settings file not found at {settingsFilePath}");
                return new GameSettings();
            }

            string json = File.ReadAllText(settingsFilePath);
            Debug.Log($"File content:\n{json}");
          
            GameSettings settings = JsonUtility.FromJson<GameSettings>(json);

            if (settings == null)
                throw new Exception("JsonUtility returned null");

            Debug.Log($"Parsed values -> PlayerHP: {settings.maxPlayerHealth}, EnemyHP: {settings.maxEnemyHealth}");
            return settings;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to load game settings: {ex.Message}");
            // Safe fallback defaults
            return new GameSettings
            {
                maxPlayerHealth = 50,
                playerDamage = 10,
                maxEnemyHealth = 50,
                enemyDamage = 10
            };
        }
    }
}