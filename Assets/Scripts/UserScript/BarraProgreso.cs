using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BarraProgresoDoom : MonoBehaviour
{
    [Header("Configuración de la barra")]
    [SerializeField] private Image _barraLlena;

    [Header("Colores")]
    public Color colorVerde = Color.green;
    public Color colorAmarillo = Color.yellow;
    public Color colorRojo = Color.red;

    [Header("Umbrales y efectos")]
    [SerializeField] private List<ThresholdEffect> _efectosPorUmbral;

    private float _progresoActual = 1f;

    public void SetProgreso(float valor)
    {
        valor = Mathf.Clamp01(valor);
        _barraLlena.fillAmount = valor;

        // Cambiar color dinámico
        if (valor > 0.5f)
        {
            float t = (valor - 0.5f) * 2f;
            _barraLlena.color = Color.Lerp(colorAmarillo, colorVerde, t);
        }
        else
        {
            float t = valor * 2f;
            _barraLlena.color = Color.Lerp(colorRojo, colorAmarillo, t);
        }

        // Evaluar efectos por umbral
        foreach (var efecto in _efectosPorUmbral)
        {
            if (!efecto.activado && valor <= efecto.umbral)
            {
                efecto.Activar();
            }
            else if (efecto.activado && valor > efecto.umbral)
            {
                efecto.Desactivar();
            }
        }

        _progresoActual = valor;
    }

    // Ejemplo de reducción progresiva (quitar para tu juego real)
    private void Update()
    {
        if (Input.GetKey(KeyCode.DownArrow))
            SetProgreso(_progresoActual - Time.deltaTime * 0.2f);

        if (Input.GetKey(KeyCode.UpArrow))
            SetProgreso(_progresoActual + Time.deltaTime * 0.2f);
    }

    [System.Serializable]
    public class ThresholdEffect
    {
        [Range(0f, 1f)]
        public float umbral; // Porcentaje (ej. 0.75 para 75%)
        public List<GameObject> spritesAActivar;
        [HideInInspector] public bool activado = false;

        public void Activar()
        {
            foreach (var go in spritesAActivar)
                if (go != null)
                    go.SetActive(true);
            activado = true;
        }

        public void Desactivar()
        {
            foreach (var go in spritesAActivar)
                if (go != null)
                    go.SetActive(false);
            activado = false;
        }
    }
}
