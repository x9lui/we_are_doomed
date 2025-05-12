using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    public Slider _audioMusica;
    public Slider _audioBotones;
    public Slider _audioEfectos;
    public Slider _sensibilidad;

    void Start()
    {
        if (_audioMusica != null && _audioBotones != null && _audioEfectos != null && _sensibilidad != null){
            Initialize();
        }
    }

    public void Initialize()
    {
        // Cargar valores actuales
        _audioMusica.value = SettingsManager.Instance.audioMusica;
        _audioBotones.value = SettingsManager.Instance.audioBotones;
        _audioEfectos.value = SettingsManager.Instance.audioEfectos;
        _sensibilidad.value = SettingsManager.Instance.sensibilidad;

        // Asignar listeners
        _audioMusica.onValueChanged.AddListener(SettingsManager.Instance.SetAudioMusica);
        Debug.Log("AudioMusica: " + SettingsManager.Instance.audioMusica);
        _audioBotones.onValueChanged.AddListener(SettingsManager.Instance.SetAudioBotones);
        _audioEfectos.onValueChanged.AddListener(SettingsManager.Instance.SetAudioEfectos);
        _sensibilidad.onValueChanged.AddListener(SettingsManager.Instance.SetSensibilidad);
    }

    void OnDestroy()
    {
        // Limpiar listeners para evitar errores si se destruye el objeto
        _audioMusica.onValueChanged.RemoveAllListeners();
        _audioBotones.onValueChanged.RemoveAllListeners();
        _audioEfectos.onValueChanged.RemoveAllListeners();
        _sensibilidad.onValueChanged.RemoveAllListeners();
    }
}
