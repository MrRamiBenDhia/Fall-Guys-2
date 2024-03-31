using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class MyMirrorNetworkScript : NetworkBehaviour 
{
    //public let[] listToDisable = new var[0];
    // Start is called before the first frame update
    public GameObject parent;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
        {

            //foreach (GameObject item in listToDisable)
            //{
            //    item.SetActive(false);
            //}
            parent.SetActive(false);
            return;
        }
    }
}
