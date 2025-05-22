using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class MouseLook : MonoBehaviour {

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; 
        Cursor.visible = false;

        // Configurar el slider si está asignado
        if (sensitivitySlider != null)
        {
            sensitivity = SettingsManager.Instance.sensibilidad;

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

    [SerializeField] private Transform playerBody;

    Vector2 rotation = Vector2.zero;
    const string xAxis = "Mouse X";
    const string yAxis = "Mouse Y";

    void Update(){
        float mouseX = Input.GetAxis(xAxis) * sensitivity;
        float mouseY = Input.GetAxis(yAxis) * sensitivity;

        rotation.x += mouseX;
        rotation.y += mouseY;
        rotation.y = Mathf.Clamp(rotation.y, -yRotationLimit, yRotationLimit);

        var yQuat = Quaternion.AngleAxis(rotation.y, Vector3.left);
        transform.localRotation = yQuat;

        playerBody.Rotate(Vector3.up * mouseX);
    }
    
    public void SetSensitivity(float value)
    {
        sensitivity = value;

        PlayerPrefs.SetFloat("MouseSensitivity", value);
        PlayerPrefs.Save();
    }

}