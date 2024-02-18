using Ilumisoft.HealthSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Sirenix.OdinInspector;


public class DamageAcript : MonoBehaviour
{
    public Health health;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ContextMenu("Damage")]
    void addDamage()
    {
        health.ApplyDamage(10);
    }
    [ContextMenu("Health")]
    void addHealth()
    {
        health.AddHealth(10);
    }

    private void OnTriggerEnter(Collider other)
    {
        addDamage();    
    }
}
