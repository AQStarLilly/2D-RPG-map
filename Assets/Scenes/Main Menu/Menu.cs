using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public static int playerCount = 1; // default to single-player

    public void PlaySinglePlayer()
    {
        playerCount = 1;
        SceneManager.LoadScene(1);
    }

    public void PlayTwoPlayer()
    {
        playerCount = 2;
        SceneManager.LoadScene(1);
    }
    
    public void Exit()
    {
        Application.Quit();
    }
}
