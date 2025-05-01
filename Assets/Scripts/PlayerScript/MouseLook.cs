using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// A simple FPP (First Person Perspective) camera rotation script.
/// Like those found in most FPS (First Person Shooter) games.
/// </summary>
public class MouseLook : MonoBehaviour {

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the center of the screen
        Cursor.visible = false; // Hide the cursor
    }
    public float Sensitivity {
        get { return sensitivity; }
        set { sensitivity = value; }
    }
    [Range(0.1f, 9f)][SerializeField] float sensitivity = 2f;
    [Tooltip("Limits vertical camera rotation. Prevents the flipping that happens when rotation goes above 90.")]
    [Range(0f, 90f)][SerializeField] float yRotationLimit = 88f;

    [SerializeField] private Transform playerBody; // Reference to the player's body for horizontal rotation

    Vector2 rotation = Vector2.zero;
    const string xAxis = "Mouse X"; // Strings in direct code generate garbage, storing and re-using them creates no garbage
    const string yAxis = "Mouse Y";

    void Update(){
        // Get mouse input
        float mouseX = Input.GetAxis(xAxis) * sensitivity;
        float mouseY = Input.GetAxis(yAxis) * sensitivity;

        // Update rotation
        rotation.x += mouseX;
        rotation.y += mouseY;
        rotation.y = Mathf.Clamp(rotation.y, -yRotationLimit, yRotationLimit);

        // Apply vertical rotation to the camera
        var yQuat = Quaternion.AngleAxis(rotation.y, Vector3.left);
        transform.localRotation = yQuat;

        // Apply horizontal rotation to the player body
        playerBody.Rotate(Vector3.up * mouseX);
    }
}