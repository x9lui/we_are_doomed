using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public GameObject gunHitEffect; // Reference to the gun hit effect prefab
    public EnemyManager enemyManager;
    public float enemyHealth;
    private Animator spriteAnim;
    private AngleToPlayer angleToPlayer;

    [Header("Attack Settings")]
    public float attackDamage = 10f; // Daño que inflige el enemigo
    public float attackCooldown = 1.5f; // Tiempo entre ataques
    private float lastAttackTime; // Tiempo del último ataque

    private PlayerHealth playerHealth; // Referencia al componente PlayerHealth

    void Start()
    {
        spriteAnim = GetComponentInChildren<Animator>(); // Obtener el Animator del enemigo
        angleToPlayer = GetComponent<AngleToPlayer>(); // Obtener el componente AngleToPlayer
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>(); // Obtener el PlayerHealth del jugador
    }

    void Update()
    {
        spriteAnim.SetFloat("SpriteRotation", angleToPlayer.lastIndex); // Convert lastIndex to float and set the sprite rotation
    }

    public void TakeDamage(float damage)
    {
        enemyHealth -= damage; // Reduce the enemy's health by the damage amount
        Instantiate(gunHitEffect, transform.position, Quaternion.identity); // Instantiate the gun hit effect at the enemy's position
        Debug.Log($"Enemy took damage: {damage}. Remaining health: {enemyHealth}"); // Log the damage taken and remaining health
        if(enemyHealth <= 0){
            Destroy(gameObject); // Destroy the enemy game object
        }
    }

    public void AttackPlayer()
    {
        // Verificar si el ataque está en cooldown
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time; // Actualizar el tiempo del último ataque

            // Activar la animación de ataque
            if (spriteAnim != null)
            {
                spriteAnim.SetTrigger("Attack");
            }

            // Infligir daño al jugador
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
                Debug.Log($"Player took {attackDamage} damage from enemy.");
            }
        }
    }

    public void Knockback(Vector3 force)
    {
        var agent = GetComponent<NavMeshAgent>();
        var rb = GetComponent<Rigidbody>();

        if (agent != null && rb != null)
        {
            agent.enabled = false; // Desactivar navegación
            rb.isKinematic = false; // Permitir física

            float knockbackMultiplier = 0.1f; // Ajusta este valor para más o menos fuerza
            rb.AddForce(force * knockbackMultiplier, ForceMode.Impulse);

            // Reactivar navegación tras un breve tiempo
            StartCoroutine(ReenableNavMeshAgent(0.5f));
        }
    }

    private IEnumerator ReenableNavMeshAgent(float delay)
    {
        yield return new WaitForSeconds(delay);
        var agent = GetComponent<NavMeshAgent>();
        var rb = GetComponent<Rigidbody>();
        if (agent != null && rb != null)
        {
            rb.linearVelocity = Vector3.zero; // ¡Esto sí detiene el movimiento!
            rb.isKinematic = true; // Volver a modo kinematic
            agent.enabled = true;
            agent.Warp(transform.position); // Recolocar el agente en la posición actual
        }
    }
}
