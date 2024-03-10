using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinBox : MonoBehaviour
{
    public GameLogic gameLogic;
    private void OnTriggerEnter(Collider other)
    {
        if (other != null)
            if (other.tag == "Player")
            {
                gameLogic.youWin();
            }
    }
}
