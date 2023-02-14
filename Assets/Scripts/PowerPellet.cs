using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerPellet : MonoBehaviour
{
    PlayerAI player;

    void Start()
    {

        player = GameObject.Find("Player").GetComponent<PlayerAI>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            print("Power pellet collected!");
            player.ActivateInvincibility();
            Destroy(gameObject);
        }
    }
}
