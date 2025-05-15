using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// A simple FPP (First Person Perspective) camera rotation script.
/// Like those found in most FPS (First Person Shooter) games.
/// </summary>
public class MouseLook : MonoBehaviour {

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the center of the screen
        Cursor.visible = false; // Hide the cursor

        // Configurar el slider si está asignado
        if (sensitivitySlider != null)
        {
            // Leer sensibilidad previa si existe
            if (PlayerPrefs.HasKey("MouseSensitivity"))
            {
                sensitivity = PlayerPrefs.GetFloat("MouseSensitivity");
            }

            sensitivitySlider.minValue = 0.1f;
            sensitivitySlider.maxValue = 10f;
            sensitivitySlider.value = sensitivity;

            // Escuchar cambios del slider
            sensitivitySlider.onValueChanged.AddListener(SetSensitivity);
        }
    }
    public float Sensitivity
    {
        get => sensitivity;
        set => SetSensitivity(value);
    }
    [Range(0.1f, 10f)][SerializeField] float sensitivity = 2f;
    [SerializeField] private Slider sensitivitySlider; // Referencia al Slider de la UI

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
    
    public void SetSensitivity(float value)
    {
        sensitivity = value;

        // Guardar sensibilidad
        PlayerPrefs.SetFloat("MouseSensitivity", value);
        PlayerPrefs.Save();
    }

}