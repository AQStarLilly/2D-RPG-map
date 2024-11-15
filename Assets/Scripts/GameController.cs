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
        player1.SetActive(true);

        if (Menu.playerCount == 2)
        {
            player2.SetActive(true);
        }
        else
        {
            player2.SetActive(false);
        }
    }
}
