using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelScript : MonoBehaviour
{
    public bool active = false;

    // Start is called before the first frame update
    void Start()
    {
        //gameObject.SetActive(active);

    }

    public void toggleUIScreen()
    {
        active = !active;
        gameObject.SetActive(active);
        //gameObject.
    }
    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            //toggleUIScreen();
        }


    }
}
