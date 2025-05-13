using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private EnemyAwareness enemyAwareness; // Reference to the EnemyAwareness component
    private Transform playersTransform; // Reference to the player's transform
    private NavMeshAgent enemyNavMeshAgent; // Reference to the NavMeshAgent component
    public void Start()
    {
        enemyAwareness = GetComponent<EnemyAwareness>(); // Get the EnemyAwareness component attached to the enemy
        playersTransform = GameObject.FindGameObjectWithTag("Player").transform; // Find the player's transform by tag
        enemyNavMeshAgent = GetComponent<NavMeshAgent>(); // Get the NavMeshAgent component attached to the enemy
    }

    private void Update()
    {
        if(enemyAwareness.isAgro) // Check if the enemy is aware of the player
        {
            // Move the enemy towards the player's position
            enemyNavMeshAgent.SetDestination(playersTransform.position); // Set the destination of the NavMeshAgent to the player's position
        }else
        {
            // Stop the enemy's movement when not aware of the player
            enemyNavMeshAgent.SetDestination(transform.position);
        }
    }
}
