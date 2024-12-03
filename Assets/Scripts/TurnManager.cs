using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public enum TurnState
    {
        PlayerTurn,
        EnemyTurn
    }

    public TurnState currentTurn = TurnState.PlayerTurn;

    public PlayerController playerController;
    public EnemyController enemyController;

    private void Start()
    {
        StartPlayerTurn();
    }

    private void StartPlayerTurn()
    {
        Debug.Log("Player's turn starts");
        currentTurn = TurnState.PlayerTurn;
        playerController.StartTurn(); // Explicitly allow the player to act
    }

    public void EndPlayerTurn()
    {
        Debug.Log("Player's turn ends");
        StartEnemyTurn();
    }

    private void StartEnemyTurn()
    {
        Debug.Log("Enemy's turn starts");
        currentTurn = TurnState.EnemyTurn;
        enemyController.StartTurn(); // Explicitly allow the enemy to act
    }

    public void EndEnemyTurn()
    {
        Debug.Log("Enemy's turn ends");
        StartPlayerTurn();
    }
}