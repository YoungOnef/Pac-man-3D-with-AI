using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    ScriptManager scriptManager;
    void Start()
    {
        scriptManager = GameObject.Find("ScriptManager").GetComponent<ScriptManager>();
    }
    private void OnTriggerEnter(Collider other)
    {
        
        
        if (other.CompareTag("Player"))
        {

            scriptManager.IncrementCoinCount();
            Destroy(gameObject);
            
        }
    }
}

