using UnityEngine;
using UnityEngine.TextCore.Text;

public class PlayerMove : MonoBehaviour
{
    public float speed = 20f; // Speed of the player movement
    private CharacterController MyCC; // Reference to the CharacterController component
    private Vector3 inputvector; // Direction of movement
    private Vector3 movementVector; // Movement vector
    private float myGravity = -9.81f; // Gravity value
    public Animator camAnim; // Reference to the Animator component
    private bool isWalking; // Flag to check if the player is walking
    void Start()
    {
        MyCC = GetComponent<CharacterController>(); // Get the CharacterController component attached to the player
    }

    // Update is called once per frame
    void Update()
    {
        GetInput(); // Call the method to get player input
        MovePlayer(); // Call the method to move the player 
        CheckForHeadBob(); // Call the method to check for head bobbing
    }

    void GetInput()
    {
        inputvector = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")); // Get input from the player
        inputvector.Normalize(); // Normalize the input vector to ensure consistent movement speed
        inputvector = transform.TransformDirection(inputvector); // Transform the input vector to world space
        movementVector = (inputvector * speed) + (Vector3.up * myGravity); // Calculate the movement vector based on input and speed 
    
    }

    void MovePlayer()
    {
        MyCC.Move(movementVector * Time.deltaTime); // Move the player using the CharacterController component
        if (MyCC.isGrounded) // Check if the player is grounded
        {
            myGravity = -9.81f; // Reset gravity when grounded
        }
        else
        {
            myGravity -= 9.81f * Time.deltaTime; // Apply gravity when not grounded
        }
    }

    void CheckForHeadBob()
    {
        if (MyCC.velocity.magnitude > 0) // Check if the player is moving
        {
            isWalking = true; // Set walking flag to true
        }
        else
        {
            isWalking = false; // Set walking flag to false
        }
        camAnim.SetBool("isWalking", isWalking); // Set the walking animation parameter in the Animator

    }
}
