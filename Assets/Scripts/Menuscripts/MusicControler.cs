using UnityEngine;

public class MusicaControlador : MonoBehaviour
{
    public AudioClip musicaFondo;

    void Start()
    {
        AudioManager.Instance.ReproducirMusica(musicaFondo);
    }
}
