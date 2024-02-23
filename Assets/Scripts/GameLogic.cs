using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    public GameObject player;
    public GameObject winPanel;
    public GameObject diePanel;

    private Vector3 originalPlayerPosition;
    private Quaternion originalPlayerRotation;

    public void gameOver()
    {
        Debug.Log("GAME OVER");
        diePanel.SetActive(true);
    }

    public void youWin()
    {
        Debug.Log("You Win");
        winPanel.SetActive(true);
    }

    public void restartGame()
    {
        player.transform.position = originalPlayerPosition;
        player.transform.rotation = originalPlayerRotation;
    }

    void Start()
    {
        if (player == null)
        {
            player = gameObject;
        }
        originalPlayerPosition = player.transform.position;
        originalPlayerRotation = player.transform.rotation;
    }

    void Update()
    {
    }
}
