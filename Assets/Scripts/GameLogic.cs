using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    GameObject player;

    public void gameOver()
    {
        Debug.Log("GAME OVER");
    }

    public void youWin()
    {
        Debug.Log("You Win");

    }

    void checkHealth()
    {
        if (player != null)
        {

        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
