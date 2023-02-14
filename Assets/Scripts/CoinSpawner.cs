using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CoinSpawner : MonoBehaviour
{
    int numToSpawn = 25;
    public float spawnOffset = 3;
    public float currentSpawnOffset = 3;

    private void Start()
    {
        if (gameObject.tag == "Coin")
        {
            currentSpawnOffset = spawnOffset;
            for (int i = 0; i < numToSpawn; i++)
            {
                GameObject clone = Instantiate(gameObject, transform.position + new Vector3(Random.Range(-spawnOffset, spawnOffset), 0, Random.Range(-spawnOffset, spawnOffset)), Quaternion.identity);
            }
        }
    }
}