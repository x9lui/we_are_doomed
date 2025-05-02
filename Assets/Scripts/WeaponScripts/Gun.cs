using System;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("Gun Settings")]
    public float damage = 10f; // Daño que inflige el arma
    public float range = 100f; // Alcance del arma
    public float fireRate = 0.5f; // Velocidad de disparo (tiempo entre disparos)
    public int ammo = 30; // Cantidad de munición actual
    public int maxAmmo = 30; // Capacidad máxima de munición

    [Header("References")]
    public AudioSource gunSound; // Sonido del disparo

    private float nextTimeToFire = 0f; // Tiempo hasta el próximo disparo permitido

    void Start()
    {
        // if (fpsCamera == null)
        // {
        //     fpsCamera = Camera.main; // Asignar la cámara principal si no está configurada
        // }
    }

    void Update()
    {
        // Detectar si el jugador presiona el botón de disparo
        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + fireRate; // Calcular el tiempo para el próximo disparo
            Fire(); // Llamar al método disparar
        }
    }

    public string GetName()
    {
        return gameObject.name; // Devolver el nombre del objeto del arma
    }

    public virtual void Fire()
    {
        // if (ammo <= 0)
        // {
        //     Debug.Log("Out of ammo!");
        //     return; // No disparar si no hay munición
        // }

        // ammo--; // Reducir la munición
        // Debug.Log($"Shot fired! Ammo left: {ammo}");

        // // Efectos visuales y de sonido
        // if (muzzleFlash != null) muzzleFlash.Play();
        // if (gunSound != null) gunSound.Play();

        // // Realizar un raycast para detectar impactos
        // RaycastHit hit;
        // if (Physics.Raycast(fpsCamera.transform.position, fpsCamera.transform.forward, out hit, range))
        // {
        //     Debug.Log($"Hit: {hit.collider.name}");

        //     // Aplicar daño si el objeto impactado tiene un componente de salud
        //     var target = hit.collider.GetComponent<Health>();
        //     if (target != null)
        //     {
        //         target.TakeDamage(damage);
        //     }
        // }
    }
}
