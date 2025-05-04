using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class AngleToPlayer : MonoBehaviour
{

    private Transform player; // Reference to the player's transform
    private Vector3 targetPos; // Position of the player
    private Vector3 targetDir; // Direction from the enemy to the player
    
    private float angle;
    public int lastIndex;
    private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer component
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // Find the player's transform by tag
        spriteRenderer = GetComponentInChildren<SpriteRenderer>(); // Get the SpriteRenderer component attached to the enemy
    }

    // Update is called once per frame
    void Update()
    {
        targetPos = new Vector3(player.position.x, transform.position.y, player.position.z); // Get the player's position
        targetDir = targetPos - transform.position; // Calculate the direction from the enemy to the player

        //Get angle
        angle = Vector3.SignedAngle(targetDir, transform.forward, Vector3.up); // Calculate the angle between the enemy's forward direction and the direction to the player

        //Flip sprites if needed

        UnityEngine.Vector3 tempScale = UnityEngine.Vector3.one; // Create a temporary scale vector
        if(angle >0){
            tempScale.x = -1; // Flip the sprite horizontally if the angle is positive
        }

        spriteRenderer.transform.localScale = tempScale; // Apply the scale to the sprite renderer
        lastIndex = GetIndex(angle); // Get the index based on the angle

    }

    private int GetIndex(float angle) {
        //front
        if(angle > -22.5f && angle < 22.6f){
            return 0;
        }
        if(angle >= 22.5f && angle < 67.5f){
            return 7;
        }
        if(angle >= 67.5f && angle < 112.5f){
            return 6;
        }
        if(angle >= 112.5f && angle < 157.5f){
            return 5;
        }

        //back
        if(angle <= -157.5 || angle >= 157.5f){
            return 4;
        }
        if(angle >= -157.4f && angle < -112.5f){
            return 3;
        }
        if(angle >= -112.5f && angle < -67.5f){
            return 2;
        }
        if(angle >= -67.5f && angle < -22.5f){
            return 1;
        }
        return lastIndex;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.forward);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, targetPos); // Draw a line from the enemy to the player    
    }

}
