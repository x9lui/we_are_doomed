using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;

    [Range(0f, 1)] public float audioMusica = 1f;
    [Range(0f, 1)] public float audioBotones = 1f;
    [Range(0f, 1)] public float audioEfectos = 1f;
    [Range(0.1f, 10f)] public float sensibilidad = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            Debug.LogWarning("Instancia duplicada de SettingsManager destruida.");
        }
    }

    // Métodos para actualizar valores
    public void SetAudioMusica(float value) => audioMusica = value;
    public void SetAudioBotones(float value) => audioBotones = value;
    public void SetAudioEfectos(float value) => audioEfectos = value;
    public void SetSensibilidad(float value) => sensibilidad = value;
}
