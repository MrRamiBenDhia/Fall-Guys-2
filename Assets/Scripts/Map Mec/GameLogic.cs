using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLogic : MonoBehaviour
{
    public GameObject player;
    public GameObject winPanel;
    public GameObject diePanel;

    public BoatScript boatScript;

    //public GameObject energyCounter;
    public TextMeshProUGUI textMesh;

    private Vector3 originalPlayerPosition;
    private Quaternion originalPlayerRotation;

    public int energyCount = 0;
    public int maxEnergy = 3;

    
    public void increaceEnergyCounter()
    {
        energyCount++;
        Debug.Log("Up 0");
        if (textMesh != null)
        {
        Debug.Log("Up one");
            textMesh.text = energyCount+"/"+ maxEnergy;
        }

        if ( maxEnergy == energyCount)
        {
            triggerBoatCanMove();
        }
    }

    public void triggerBoatCanMove()
    {
        if (boatScript != null)
        {
            boatScript.canMoving = true;
        }
    }
    public void triggerBoatMove()
    {
        if (boatScript != null)
        {
            boatScript.StartMovement();
        }
    }

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
