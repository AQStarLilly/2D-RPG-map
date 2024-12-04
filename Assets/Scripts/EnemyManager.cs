using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance; // Singleton instance
    private Dictionary<Vector3Int, EnemyController> enemyPositions = new Dictionary<Vector3Int, EnemyController>();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void RegisterEnemy(Vector3Int position, EnemyController enemy)
    {
        if (!enemyPositions.ContainsKey(position))
        {
            enemyPositions[position] = enemy;
            Debug.Log($"Enemy registered at {position}");
        }
    }

    public void UnregisterEnemy(Vector3Int position)
    {
        if (enemyPositions.ContainsKey(position))
        {
            enemyPositions.Remove(position);
        }
    }

    public EnemyController GetEnemyAtPosition(Vector3Int position)
    {
        if (enemyPositions.TryGetValue(position, out EnemyController enemy))
        {
            Debug.Log($"Enemy found at position {position}");
            return enemy;
        }
        Debug.Log($"No enemy at position {position}");
        return null;
    }
}
