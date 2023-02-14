using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class PlayerAI : MonoBehaviour
{

    public bool invincibilityActive = false;
    public float speed = 10.0f;
    public NavMeshAgent agent;
    public float searchRadius = 10f;
    private Rigidbody rigidbody;
    public TextMeshProUGUI stateText;
    public float chaseDistance = 5.0f;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        
    }
    private void Update()
    {
        if (IsCoinClose())
        {
            FindNextTarget("Coin");
        }
        else if (IsPowerPelletClose())
        {
            FindNextTarget("PowerPallet");
        }
        else if (AreEnemiesClose())
        {
            if (invincibilityActive)
            {
                Attack();
            }
            else
            {
                RunAwayFromEnemies();
            }
        }
    }

    private bool IsCoinClose()
    {
        GameObject[] coins = GameObject.FindGameObjectsWithTag("Coin");
        float shortestDistance = Mathf.Infinity;

        // Find the nearest coin
        foreach (GameObject coin in coins)
        {
            float distance = Vector3.Distance(transform.position, coin.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
            }
        }

        if (shortestDistance <= searchRadius)
        {
            return true;
        }
        return false;
    }

    private bool IsPowerPelletClose()
    {
        GameObject[] powerPellets = GameObject.FindGameObjectsWithTag("PowerPallet");
        float shortestDistance = Mathf.Infinity;

        // Find the nearest power pellet
        foreach (GameObject powerPellet in powerPellets)
        {
            float distance = Vector3.Distance(transform.position, powerPellet.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
            }
        }

        if (shortestDistance <= searchRadius)
        {
            return true;
        }
        return false;
    }

    private void FindNextTarget(string targetTag)
    {
        stateText.text = "Finding the target";
        GameObject[] targets = GameObject.FindGameObjectsWithTag(targetTag);
        float shortestDistance = Mathf.Infinity;
        GameObject bestTarget = null;

        foreach (GameObject target in targets)
        {
            float distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                bestTarget = target;
            }
        }
        if (bestTarget != null)
        {
            agent.destination = bestTarget.transform.position;
            stateText.text = "Moving towards the target";
        }
        else
        {
            stateText.text = "No target found";
        }
    }


    private bool AreEnemiesClose()
    {
        // Get all enemies in the scene
        Enemy[] enemies = FindObjectsOfType<Enemy>();

        // Check if the player is near any of the enemies
        foreach (Enemy enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < chaseDistance)
            {
                return true;
            }
        }
        return false;
    }


    private float CalculateScore(float distance, string targetTag)
    {
        float proximityScore = 1 / (distance + 1); // +1 to prevent division by zero
        int targetCount = GameObject.FindGameObjectsWithTag(targetTag).Length;
        float completionScore = (targetCount - 1) / targetCount; // -1 to exclude the current target

        return proximityScore + completionScore;
    }
    private void RunAwayFromEnemies()
    {
        stateText.text = "Running Away From Enemies";
        // Get all enemies in the scene
        Enemy[] enemies = FindObjectsOfType<Enemy>();

        // Check if the player is near any of the enemies
        foreach (Enemy enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < chaseDistance)
            {
                // Set the destination to a point away from the enemy
                Vector3 awayDirection = transform.position - enemy.transform.position;
                Vector3 runawayDestination = transform.position + awayDirection;
                agent.SetDestination(runawayDestination);
                return;
            }
        }
    }
    private void Attack()
    {
        stateText.text = "Attack Enemy";
        // Get all enemies in the scene
        Enemy[] enemies = FindObjectsOfType<Enemy>();

        float shortestDistance = Mathf.Infinity;
        Enemy nearestEnemy = null;

        // Find the nearest enemy
        foreach (Enemy enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestEnemy = enemy;
            }
        }

        // Move towards the nearest enemy
        if (nearestEnemy != null)
        {
            agent.destination = nearestEnemy.transform.position;
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (invincibilityActive && collision.gameObject.CompareTag("Enemy"))
        {
            // Kill the enemy when the player collides with it while invincibility is active
            Destroy(collision.gameObject);
        }
    }

    public void ActivateInvincibility()
    {
        invincibilityActive = true;
        // Start a coroutine to deactivate invincibility after a set amount of time
        StartCoroutine(DeactivateInvincibility());
    }

    private IEnumerator DeactivateInvincibility()
    {
        yield return new WaitForSeconds(10f);
        invincibilityActive = false;
    }
}