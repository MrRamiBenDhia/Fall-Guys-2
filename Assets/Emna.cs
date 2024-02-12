using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Emna : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);   
        
    }

    private void OnCollisionEnter(Collision collision)
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
