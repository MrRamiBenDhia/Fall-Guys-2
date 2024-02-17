using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelItem : MonoBehaviour
{
    public PlatformScript platformScript;
    public PanelScript panelScript;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void ActivateScript()
    {
        platformScript.active = true;
    }
    void ActivatePanel()
    {
        //panelScript.active = true;
        //panelScript.toggleUIScreen();
        panelScript.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (platformScript)
        {
            ActivateScript();

        }        
        if (panelScript)
        {
            ActivatePanel();

        }
        Destroy(gameObject);
        
    }
    private void OnCollisionEnter(Collision collision)
    {
    }
}
