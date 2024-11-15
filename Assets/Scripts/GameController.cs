using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject player1;
    public GameObject player2;

    // Start is called before the first frame update
    void Start()
    {
        player1.SetActive(true);  //single player default

        if (Menu.playerCount == 2) //if 2 player option chosen
        {
            player2.SetActive(true);  //sets 2nd player active
        }
        else
        {
            player2.SetActive(false);  //keeps 2nd player inactive
        }
    }
}
