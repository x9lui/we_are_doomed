using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private EnemyAwareness enemyAwareness; // Referencia al componente EnemyAwareness
    private Transform playersTransform; // Referencia al transform del jugador
    private NavMeshAgent enemyNavMeshAgent; // Referencia al componente NavMeshAgent
    private Enemy enemyScript; // Referencia al script Enemy

    [Header("Enemy Settings")]
    public float attackRange = 2.0f; // Distancia mínima para atacar

    public void Start()
    {
        enemyAwareness = GetComponent<EnemyAwareness>(); // Obtener el componente EnemyAwareness
        playersTransform = GameObject.FindGameObjectWithTag("Player").transform; // Encontrar el transform del jugador por su etiqueta
        enemyNavMeshAgent = GetComponent<NavMeshAgent>(); // Obtener el componente NavMeshAgent
        enemyScript = GetComponent<Enemy>(); // Obtener el componente Enemy
    }

    private void Update()
    {
        if (enemyAwareness.isAgro) // Verificar si el enemigo está al tanto del jugador
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playersTransform.position); // Calcular la distancia al jugador

            if (distanceToPlayer > attackRange)
            {
                // Mover al enemigo hacia el jugador si está fuera del rango de ataque
                enemyNavMeshAgent.SetDestination(playersTransform.position);
            }
            else
            {
                // Detener al enemigo si está dentro del rango de ataque
                enemyNavMeshAgent.SetDestination(transform.position);

                // Llamar al método de ataque del script Enemy
                enemyScript.AttackPlayer();
            }
        }
        else
        {
            // Detener al enemigo si no está al tanto del jugador
            enemyNavMeshAgent.SetDestination(transform.position);
        }
    }
}
