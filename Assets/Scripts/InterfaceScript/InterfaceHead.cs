using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InterfaceHead : MonoBehaviour
{
    [Header("Imagen de la cabeza del HUD")]
    [SerializeField] private Image imagenCabeza;

    [Header("Rangos de armadura")]
    [SerializeField] private List<RangoEstado> rangosArmadura;

    [Header("Rangos de salud")]
    [SerializeField] private List<RangoEstado> rangosSalud;

    private float saludActual = 1f;  // entre 0 y 1
    private float armaduraActual = 1f;  // entre 0 y 1

    public void SetSalud(float nuevaSalud)
    {
        saludActual = Mathf.Clamp01(nuevaSalud);
        ActualizarSprite();
    }

    private void Update(){
        ActualizarSprite();
    }

    public void SetArmadura(float nuevaArmadura)
    {
        armaduraActual = Mathf.Clamp01(nuevaArmadura);
        ActualizarSprite();
    }

    private void ActualizarSprite()
    {
        List<RangoEstado> rangosAUsar = armaduraActual > 0 ? rangosArmadura : rangosSalud;
        float valorActual = armaduraActual > 0 ? armaduraActual : saludActual;

        foreach (var rango in rangosAUsar)
        {
            if (valorActual <= rango.maxSalud && valorActual > rango.minSalud)
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
