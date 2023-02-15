using UnityEngine;
using UnityEngine.AI;
using TMPro;
using static UnityEngine.GraphicsBuffer;

public class Enemy : MonoBehaviour
{
    public Transform target; // target is the position of the player
    NavMeshAgent agent;
    PlayerAI player;
    bool hasReachedDestination;
    public TextMeshProUGUI stateText;
    float chaseDistance = 5.0f;
    private float stuckTime = 0f;
    private float stuckThreshold = 2f;



    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = FindObjectOfType<PlayerAI>();
        hasReachedDestination = false;
    }

    private void Update()
    {
        // Root node of the decision tree
        if (player.invincibilityActive)
        {
            // Decision 1: If the player is invulnerable, run away
            RunAway();
            stateText.text = "Running away";
        }
        else
        {
            // Decision 2: If the player is not invulnerable, check if the player is within chase distance
            if (Vector3.Distance(transform.position, target.position) < chaseDistance)
            {
                // Decision 2.1: If the player is within chase distance, chase the player
                agent.SetDestination(target.position);
                stateText.text = "Chase";
            }
            else
            {
                // Decision 2.2: If the player is not within chase distance, check if the agent has a path or if it's stuck
                if (!agent.hasPath || agent.remainingDistance < 0.1f)
                {
                    // Decision 2.2.1: If the agent doesn't have a path or is stuck, set a random location
                    NavMeshHit hit;
                    NavMesh.SamplePosition(transform.position + Random.onUnitSphere * chaseDistance, out hit, chaseDistance, NavMesh.AllAreas);
                    agent.SetDestination(hit.position);
                    stateText.text = "Idle";
                    stuckTime = 0f;
                }
                else
                {
                    // Decision 2.2.2: If the agent has a path, check if it's stuck
                    stuckTime += Time.deltaTime;
                    if (stuckTime > stuckThreshold)
                    {
                        // Decision 2.2.2.1: If the agent is stuck, set a new random location
                        NavMeshHit hit;
                        NavMesh.SamplePosition(transform.position + Random.onUnitSphere * chaseDistance, out hit, chaseDistance, NavMesh.AllAreas);
                        agent.SetDestination(hit.position);
                        stuckTime = 0f;
                    }
                }
            }
        }
    }

    private void RunAway()
    {
        // Set the destination to a point away from the player
        Vector3 awayDirection = transform.position - target.position;
        Vector3 runawayDestination = transform.position + awayDirection;
        agent.SetDestination(runawayDestination);
    }

    private void OnTriggerEnter(Collider other)
    {
        //print("OnTriggerEnter Enemy Triggere" + other.gameObject.name);
        if (!player.invincibilityActive && other.CompareTag("Player"))
        {
            // Handle collision with player
            // e.g. reduce player's health, trigger game over, etc.
            Destroy(other.gameObject);
            print("You lose!");
        }
    }
}