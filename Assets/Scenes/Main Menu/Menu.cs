using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public static int playerCount = 1; // default to single-player

    public void PlaySinglePlayer()  //load game as 1 person
    {
        playerCount = 1;
        SceneManager.LoadScene(1);
    }

    public void PlayTwoPlayer()  //load as 2 players
    {
        playerCount = 2;
        SceneManager.LoadScene(1);
    }
    
    public void Exit()   //quit the game if your playing in a build
    {
        Application.Quit();
    }
}
