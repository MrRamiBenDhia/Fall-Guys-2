using Lightbug.CharacterControllerPro.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class windScript : MonoBehaviour
{

    private void OnTriggerStay(Collider other)
    {
            Debug.Log("Other");
            Debug.Log(other.name);

        if (other.tag == "Player")
        {
            Debug.Log("INN");

             other.GetComponentInChildren<CharacterBody>().RigidbodyComponent.AddForce(Vector3.forward * 10);
        }
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
