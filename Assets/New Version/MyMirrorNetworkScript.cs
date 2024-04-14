using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class MyMirrorNetworkScript : NetworkBehaviour 
{
    // Start is called before the first frame update
    public GameObject parent;
    public Component[] listToDisable;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
        {

            foreach (Component component in listToDisable)
            {
                DisableAllScripts(component.gameObject);
            }
            parent.SetActive(false);
            return;
        }
    }

    void DisableAllScripts(GameObject obj)
    {
        MonoBehaviour[] scripts = obj.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            script.enabled = false;
        }
    }
}
