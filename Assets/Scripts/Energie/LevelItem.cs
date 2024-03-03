using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelItem : MonoBehaviour
{
    public enum MyType
    {
        Energy, Win, Die, Nothing
    }

    public MyType type = MyType.Nothing;

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
        switch (type)
        {
            case MyType.Energy:
                GameLogic gameLogic = other.GetComponentInParent<GameLogic>();
                gameLogic.increaceEnergyCounter();
                break;
            //case MyType.Win:
            //    break;
            //case MyType.Die:
            //    break;
            //case MyType.Nothing:
            //    break;
        }



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

}
