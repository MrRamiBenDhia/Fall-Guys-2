using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelItem : MonoBehaviour
{
    public PlatformScript platformScript;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void ActivateScript()
    {
        platformScript.active = true;
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
        Destroy(gameObject);
        
    }
    private void OnCollisionEnter(Collision collision)
    {
    }
}
