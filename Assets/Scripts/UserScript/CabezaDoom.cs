using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CabezaDoom : MonoBehaviour
{
    [Header("Imagen de la cabeza del HUD")]
    [SerializeField] private Image imagenCabeza;

    [Header("Rangos de salud y sprites")]
    [SerializeField] private List<RangoEstado> rangos;

    private float saludActual = 1f; // entre 0 y 1

    public void SetSalud(float nuevaSalud)
    {
        saludActual = Mathf.Clamp01(nuevaSalud);
        ActualizarSprite();
    }

    private void Update()
    {
        // Ejemplo para probar: simula daño y teclas
        if (Input.GetKey(KeyCode.DownArrow))
            SetSalud(saludActual - Time.deltaTime * 0.2f);
        if (Input.GetKey(KeyCode.UpArrow))
            SetSalud(saludActual + Time.deltaTime * 0.2f);

        ActualizarSprite(); // Detectar pulsaciones en tiempo real
    }

    private void ActualizarSprite()
    {
        foreach (var rango in rangos)
        {
            if (saludActual <= rango.maxSalud && saludActual > rango.minSalud)
            {
                if (Input.GetKey(KeyCode.A))
                {
                    imagenCabeza.sprite = rango.spriteIzquierda;
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    imagenCabeza.sprite = rango.spriteDerecha;
                }
                else
                {
                    imagenCabeza.sprite = rango.spriteNeutro;
                }
                return;
            }
        }
    }

    [System.Serializable]
    public class RangoEstado
    {
        [Range(0f, 1f)]
        public float minSalud;
        [Range(0f, 1f)]
        public float maxSalud;

        public Sprite spriteNeutro;
        public Sprite spriteIzquierda;
        public Sprite spriteDerecha;
    }
}
