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
        Debug.Log("Reproduciendo efectos de sonido: " + clip.name);
        Debug.Log("Volumen de efectos: " + BotonEfectos.volume);
        Debug.Log("AudioSource de efectos: " + BotonEfectos);
        BotonEfectos.PlayOneShot(clip);
    }

    public void ReproducirEfectos2(string nombreObjeto, AudioClip clip)
    {
        GameObject tempAudio = new GameObject(nombreObjeto);
        tempAudio.transform.position = transform.position;
    
        AudioSource original = GetComponent<AudioSource>(); // El AudioSource de referencia
        if (original == null)
        {
            Debug.LogWarning("No se encontró un AudioSource de referencia en este objeto.");
            Destroy(tempAudio);
            return;
        }
    
        AudioSource audioSource = tempAudio.AddComponent<AudioSource>();
        
        // Copiamos todos los parámetros necesarios desde el original
        audioSource.clip = clip;
        audioSource.volume = original.volume;
        audioSource.pitch = original.pitch;
        audioSource.spatialBlend = original.spatialBlend;
        audioSource.loop = original.loop;
        audioSource.priority = original.priority;
        audioSource.dopplerLevel = original.dopplerLevel;
        audioSource.rolloffMode = original.rolloffMode;
        audioSource.minDistance = original.minDistance;
        audioSource.maxDistance = original.maxDistance;
        audioSource.outputAudioMixerGroup = original.outputAudioMixerGroup;
    
        audioSource.Play();
        Destroy(tempAudio, clip.length);
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
