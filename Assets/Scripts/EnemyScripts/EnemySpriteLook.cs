using UnityEngine;

public class EnemySpriteLook : MonoBehaviour
{
    private Transform target;
    public bool canLookVertically; // Flag to allow vertical rotation
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform; // Find the player's transform by tag   
    }

    // Update is called once per frame
    void Update()
    {
        if(canLookVertically){
            transform.LookAt(target); // Make the enemy look at the player
        }else{
            Vector3 modifiedTarget = target.position; // Create a modified target position
            modifiedTarget.y = transform.position.y; // Set the y-coordinate to the enemy's y-coordinate
            transform.LookAt(modifiedTarget); // Make the enemy look at the modified target
        }
    }
}
