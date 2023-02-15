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
    float maxDistance = 15f;


    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();

    }
    private void Update()
    {


        if (AreEnemiesClose())
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
        else if (IsObjectClose("PowerPallet"))
        {
            FindNextTarget("PowerPallet");
        }
        else if (IsObjectClose("Coin"))
        {
            FindNextTarget("Coin");
        }
    }
    private void FindNextTarget(string targetTag)
    {
        stateText.text = "Finding the target = " + targetTag;
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
            StartCoroutine(WaitForArrival(bestTarget));
        }
        else
        {
            stateText.text = "No target found";
        }
    }
    private IEnumerator WaitForArrival(GameObject target)
    {
        while (target != null && Vector3.Distance(transform.position, target.transform.position) > 0.5f)
        {
            yield return null;
        }
    }
    private bool IsObjectClose(string tag)
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
        float shortestDistance = Mathf.Infinity;

        // Find the nearest object
        foreach (GameObject obj in objects)
        {
            float distance = Vector3.Distance(transform.position, obj.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
            }
        }

        if (shortestDistance <= maxDistance)
        {
            return true;
        }
        return false;
    }
    private bool AreEnemiesClose()
    {
        // Find all game objects with the Enemy tag
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        // Check if the player is near any of the enemies
        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < chaseDistance)
            {
                stateText.text = "AreEnemiesClose YES";
                return true;
            }
        }
        stateText.text = "AreEnemiesClose NO";
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
        // Get all game objects with tag "Enemy"
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        // Check if the player is near any of the enemies
        foreach (GameObject enemy in enemies)
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
        GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag("Enemy");

        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        // Find the nearest enemy
        foreach (GameObject enemyObject in enemyObjects)
        {
            float distance = Vector3.Distance(transform.position, enemyObject.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestEnemy = enemyObject;
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