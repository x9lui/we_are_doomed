using UnityEngine;

public class Enemy : MonoBehaviour
{
 
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
        Debug.Log(angleToPlayer.lastIndex); // Log the last index from AngleToPlayer for debugging
        spriteAnim.SetFloat("SpriteRotation", angleToPlayer.lastIndex); // Set the sprite rotation based on the last index from AngleToPlayer
    }

    public void TakeDamage(float damage)
    {
        enemyHealth -= damage; // Reduce the enemy's health by the damage amount
        if(enemyHealth <= 0){
            // enemyManager.RemoveEnemy(this); // Remove the enemy from the game
            Destroy(gameObject); // Destroy the enemy game object
        }
    }
}
