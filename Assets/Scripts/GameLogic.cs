using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        restartScene();
    }

    public void restartScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    void Start()
    {
        if (player == null)
        {
            player = gameObject;
        }
        
    }

    void Update()
    {
    }
}
