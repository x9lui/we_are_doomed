using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Fuentes de audio")]
    public AudioSource Boton;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            AplicarVolumen(); // Aplica los volúmenes al iniciar
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // Actualiza constantemente el volumen desde SettingsManager
        AplicarVolumen();
    }

    private void AplicarVolumen()
    {
        if (SettingsManager.Instance == null) return;

        Boton.volume = SettingsManager.Instance.audioMusica;
    }

    // Métodos para reproducir clips
    public void Reproducir(AudioClip clip)
    {
        if(clip == null){
            Boton.Play();
        }
        Boton.PlayOneShot(clip);
    }
}
