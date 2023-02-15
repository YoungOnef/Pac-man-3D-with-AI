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
    private ScriptManager scriptManager;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        scriptManager = FindObjectOfType<ScriptManager>();

    }
    private void Update()
    {
        SequenceForCollectingCoins();

    }

    private void SequenceForCollectingCoins()
    {
        if (IsCoinClose())
        {
            ConditionForCheckingCoins();
            ActionForFindingTheNearestCoin();
            ActionForCollectingCoin();
        }
    }
    private bool IsCoinClose()
    {
        // Get all colliders within a sphere with a radius of `searchRadius`
        Collider[] colliders = Physics.OverlapSphere(transform.position, searchRadius);

        // Iterate through each collider
        foreach (var collider in colliders)
        {
            // Check if the collider has the tag "Coin"
            if (collider.gameObject.CompareTag("Coin"))
            {
                // Check the distance between the player and the coin
                float distance = Vector3.Distance(transform.position, collider.gameObject.transform.position);
                if (distance <= chaseDistance)
                {
                    // If the coin is close enough, return true
                    return true;
                }
            }
        }
        // If no coin was close enough, return false
        return false;
    }

    private void ConditionForCheckingCoins()
    {
        if (IsCoinClose())
        {
            stateText.text = "Coin found!";
        }
        else
        {
            stateText.text = "Searching for coins...";
        }
    }


    private void ActionForFindingTheNearestCoin()
    {
        // Add code here to find the nearest coin and set it as the destination for the NavMeshAgent
        // Example:
        GameObject nearestCoin = FindNearestCoin();
        agent.destination = nearestCoin.transform.position;
    }

    private GameObject FindNearestCoin()
    {
        // Get all colliders within a sphere with a radius of `searchRadius`
        Collider[] colliders = Physics.OverlapSphere(transform.position, searchRadius);

        // Keep track of the nearest coin
        GameObject nearestCoin = null;
        float nearestDistance = Mathf.Infinity;

        // Iterate through each collider
        foreach (var collider in colliders)
        {
            // Check if the collider has the tag "Coin"
            if (collider.gameObject.CompareTag("Coin"))
            {
                // Check the distance between the player and the coin
                float distance = Vector3.Distance(transform.position, collider.gameObject.transform.position);
                if (distance < nearestDistance)
                {
                    // If this coin is closer than the current nearest coin, update the nearest coin and distance
                    nearestCoin = collider.gameObject;
                    nearestDistance = distance;
                }
            }
        }
        // Return the nearest coin
        return nearestCoin;
    }

    private void ActionForCollectingCoin()
    {
        Coin closestCoin = GetClosestCoin();
        if (closestCoin == null)
        {
            return;
        }

        float distance = Vector3.Distance(transform.position, closestCoin.transform.position);
        if (distance <= chaseDistance)
        {
            scriptManager.IncrementCoinCount();
            Destroy(closestCoin.gameObject);
        }
    }

    private Coin GetClosestCoin()
    {
        Coin[] coins = GameObject.FindObjectsOfType<Coin>();
        Coin closestCoin = null;
        float closestDistance = float.MaxValue;

        foreach (Coin coin in coins)
        {
            float distance = Vector3.Distance(transform.position, coin.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestCoin = coin;
            }
        }

        return closestCoin;
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