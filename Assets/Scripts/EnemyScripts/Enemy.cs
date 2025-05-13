using UnityEngine;

public class Enemy : MonoBehaviour
{
 
    public GameObject gunHitEffect; // Reference to the gun hit effect prefab
    public EnemyManager enemyManager; // Reference to the EnemyManager component
    private float enemyHealth = 2f;
    private Animator spriteAnim;
    private AngleToPlayer angleToPlayer; // Reference to the AngleToPlayer component
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteAnim = GetComponentInChildren<Animator>(); // Get the Animator component attached to the enemy
        angleToPlayer = GetComponent<AngleToPlayer>(); // Get the AngleToPlayer component attached to the enemy
        // enemyManager = FindObjectOfType<EnemyManager>(); // Find the EnemyManager in the scene
    }

    void Update()
    {
        // Debug.Log(angleToPlayer.lastIndex); // Log the last index from AngleToPlayer for debugging
        spriteAnim.SetFloat("SpriteRotation", angleToPlayer.lastIndex); // Convert lastIndex to float and set the sprite rotation
    }

    public void TakeDamage(float damage)
    {
        enemyHealth -= damage; // Reduce the enemy's health by the damage amount
        Instantiate(gunHitEffect, transform.position, Quaternion.identity); // Instantiate the gun hit effect at the enemy's position
        Debug.Log($"Enemy took damage: {damage}. Remaining health: {enemyHealth}"); // Log the damage taken and remaining health
        if(enemyHealth <= 0){
            // enemyManager.RemoveEnemy(this); // Remove the enemy from the game
            Destroy(gameObject); // Destroy the enemy game object
        }
    }
}
