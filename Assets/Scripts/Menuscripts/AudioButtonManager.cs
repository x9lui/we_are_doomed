using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Fuentes de audio")]
    public AudioSource BotonMusica;
    public AudioSource BotonEfectos;
    public AudioSource BotonInterfaz;

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

        BotonMusica.volume = SettingsManager.Instance.audioMusica;
        BotonEfectos.volume = SettingsManager.Instance.audioEfectos;
        BotonInterfaz.volume = SettingsManager.Instance.audioBotones;

    }

    // Métodos para reproducir clips para Interfaz.
    public void ReproducirInterfaz(AudioClip clip)
    {
        if (clip == null)
        {
            BotonInterfaz.Play();
        }
        BotonInterfaz.PlayOneShot(clip);
    }

    // Para los sonidos de Efectos usar el BotonEfectos
    public void ReproducirEfectos(AudioClip clip)
    {
        if (clip == null)
        {
            BotonEfectos.Play();
        }
        BotonEfectos.PlayOneShot(clip);
    }

    public void ReproducirEfectos2(string Objeto, AudioClip clip)
    {
        // Crea un objeto temporal para el sonido
        GameObject tempAudio = new GameObject("TempRocketExplosionAudio");
        tempAudio.transform.position = transform.position;
        AudioSource BotonEfectos = tempAudio.AddComponent<AudioSource>();
        BotonEfectos.Play();
    }
    // Para los sonidos de la musica usar BotonMusica
    public void ReproducirMusica(AudioClip clip)
    {
        if (clip == null) return;
    
        BotonMusica.clip = clip;
        BotonMusica.loop = true; // Opcional: si quieres que la música se repita
        BotonMusica.Play();
    }

}
