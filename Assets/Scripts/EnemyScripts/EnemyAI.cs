using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private EnemyAwareness enemyAwareness;
    private Transform playersTransform;
    private NavMeshAgent enemyNavMeshAgent;
    private Enemy enemyScript;

    [Header("Enemy Settings")]
    public float attackRange = 2.0f; // Distancia mínima para atacar

    public void Start()
    {
        enemyAwareness = GetComponent<EnemyAwareness>();
        playersTransform = GameObject.FindGameObjectWithTag("Player").transform;
        enemyNavMeshAgent = GetComponent<NavMeshAgent>();
        enemyScript = GetComponent<Enemy>();
    }

    private void Update()
    {
        if (enemyAwareness.isAgro) // Verificar si el enemigo está al tanto del jugador
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playersTransform.position);

            if (distanceToPlayer > attackRange)
            {

                enemyNavMeshAgent.SetDestination(playersTransform.position);
            }
            else
            {

                enemyNavMeshAgent.SetDestination(transform.position);


                enemyScript.AttackPlayer();
            }
        }
        else
        {

            enemyNavMeshAgent.SetDestination(transform.position);
        }
    }
}
