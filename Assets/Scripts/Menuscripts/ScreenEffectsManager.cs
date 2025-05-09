using UnityEngine;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Administra efectos visuales en múltiples pantallas de UI para simular un monitor CRT antiguo.
/// </summary>
public class GestorEfectosPantalla : MonoBehaviour
{
    [System.Serializable]
    public class _ParTextoPanel
    {
        public RectTransform _panelPantalla;
        public TextMeshProUGUI _textoPantalla;
    }

    [SerializeField] private List<_ParTextoPanel> paresPantalla;

    [SerializeField] private float _magnitudTemblor = 2f;

    [SerializeField] private float _velocidadTemblor = 5f;

    [SerializeField] private float _intensidadParpadeo = 0.1f;

    [SerializeField] private float _velocidadParpadeo = 5f;

    private List<Vector3> _posicionesIniciales = new List<Vector3>();
    private List<Color> _coloresIniciales = new List<Color>();
    private Vector3 _maxDesplazamientoTemblor;

    private void Start()
    {
        if (paresPantalla != null && paresPantalla.Count > 0)
        {
            foreach (var par in paresPantalla)
            {
                if (par._panelPantalla != null)
                {
                    _posicionesIniciales.Add(par._panelPantalla.localPosition);
                }

                if (par._textoPantalla != null)
                {
                    _coloresIniciales.Add(par._textoPantalla.color);
                }
            }

            _maxDesplazamientoTemblor = new Vector3(_magnitudTemblor, _magnitudTemblor, 0f);
        }
    }

    private void Update()
    {
        AplicarEfectosPantalla();
    }

    /// <summary>
    /// Aplica efectos de sacudida y parpadeo para simular una pantalla CRT.
    /// </summary>
    private void AplicarEfectosPantalla()
    {
        for (int i = 0; i < paresPantalla.Count; i++)
        {
            if (paresPantalla[i]._panelPantalla != null)
            {
                AplicarTemblor(i);
            }
            if (paresPantalla[i]._textoPantalla != null)
            {
                AplicarParpadeo(i);
            }
        }
    }

    /// <summary>
    /// Aplica una pequeña sacudida al panel de la pantalla manteniéndolo dentro de los límites permitidos.
    /// </summary>
    private void AplicarTemblor(int indice)
    {
        Vector3 desplazamientoAleatorio = new Vector3(
            Mathf.PerlinNoise(Time.time * _velocidadTemblor, 0f) - 0.5f,
            Mathf.PerlinNoise(0f, Time.time * _velocidadTemblor) - 0.5f,
            0f
        ) * _magnitudTemblor;

        Vector3 nuevaPosicion = _posicionesIniciales[indice] + desplazamientoAleatorio;

        // Restringir el desplazamiento a los límites establecidos
        nuevaPosicion.x = Mathf.Clamp(nuevaPosicion.x, _posicionesIniciales[indice].x - _maxDesplazamientoTemblor.x, _posicionesIniciales[indice].x + _maxDesplazamientoTemblor.x);
        nuevaPosicion.y = Mathf.Clamp(nuevaPosicion.y, _posicionesIniciales[indice].y - _maxDesplazamientoTemblor.y, _posicionesIniciales[indice].y + _maxDesplazamientoTemblor.y);

        paresPantalla[indice]._panelPantalla.localPosition = nuevaPosicion;
    }

    /// <summary>
    /// Aplica un efecto de parpadeo al texto de la pantalla.
    /// </summary>
    private void AplicarParpadeo(int indice)
    {
        float parpadeo = Mathf.PerlinNoise(Time.time * _velocidadParpadeo, 0f) * _intensidadParpadeo;
        paresPantalla[indice]._textoPantalla.color = new Color(
            _coloresIniciales[indice].r,
            _coloresIniciales[indice].g,
            _coloresIniciales[indice].b,
            1f - parpadeo
        );
    }
}
