using UnityEngine;
using UnityEngine.UI;

public class SliderLinker : MonoBehaviour
{
    public Slider _SliderMusica;
    public Slider _SliderEfectos;
    public Slider _SliderInterfaz;
    public Slider _SliderSensibilidad;

    void Start()
    {
        GameObject audioManagerObj = GameObject.Find("AudioManager");
        if (audioManagerObj == null)
        {
            Debug.LogWarning("No se encontró un GameObject llamado 'AudioManager'");
            return;
        }

        SettingsUI settingsUI = audioManagerObj.GetComponent<SettingsUI>();
        if (settingsUI == null)
        {
            Debug.LogWarning("El GameObject 'AudioManager' no tiene un componente SettingsUI");
            return;
        }

        // Asignar sliders si no están asignados
        if (settingsUI._audioMusica == null && _SliderMusica != null)
            settingsUI._audioMusica = _SliderMusica;

        if (settingsUI._audioEfectos == null && _SliderEfectos != null)
            settingsUI._audioEfectos = _SliderEfectos;

        if (settingsUI._audioBotones == null && _SliderInterfaz != null)
            settingsUI._audioBotones = _SliderInterfaz;

        if (settingsUI._sensibilidad == null && _SliderSensibilidad != null)
            settingsUI._sensibilidad = _SliderSensibilidad;

        // Llamar a la inicialización si existe
        settingsUI.Initialize();
    }
}
