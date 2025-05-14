using System;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("Gun Settings")]
    public float damage; // Daño que inflige el arma
    public float fireRate; // Velocidad de disparo (tiempo entre disparos)
    public int ammo; // Cantidad de munición actual
    public int maxAmmo; // Capacidad máxima de munición
    public float RayCastDistance; // Distancia máxima del raycast

    protected RayCast rayCast; // Referencia al script RayCast

    [Header("References")]
    public AudioSource gunSound; // Sonido del disparo

    private float nextTimeToFire; // Tiempo hasta el próximo disparo permitido
    protected bool isFiring = false; // Nueva bandera para controlar si el arma está disparando

    public bool canAuto;

    protected Animator spriteAnim;

    [Obsolete]
    void Start()
    {
        spriteAnim = GetComponent<Animator>(); // Get the Animator component attached to the enemy
        rayCast = FindObjectOfType<RayCast>();
        if (rayCast == null)
        {
            Debug.LogError("RayCast script not found!");
        }

    }

    public GameObject HandleRaycastAndDamage()
    {
        // Llamar al método CheckRayCastCollisionWithEnemy del script RayCast
        GameObject enemyObject = rayCast.CheckRayCastCollisionWithEnemy();
        if (enemyObject != null)
        {
            Debug.Log($"Hit enemy: {enemyObject.name}");

            // Obtener el componente Enemy y aplicar daño
            Enemy enemy = enemyObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage); // Aplicar daño al enemigo
                return enemyObject; // Devolver el enemigo golpeado
            }
            else
            {
                Debug.LogWarning("Hit object does not have an Enemy component!");
            }
        }
        else
        {
            Debug.Log("No enemy hit");
        }

        return null; // Si no se golpeó a ningún enemigo
    }

    public virtual void Fire()
    {
        if (isFiring || Time.time < nextTimeToFire)
        {
            Debug.Log("Gun is not ready to fire.");
            return;
        }

        isFiring = true; // Marcar el arma como disparando
        nextTimeToFire = Time.time + fireRate; // Configurar el tiempo para el próximo disparo
    }

    protected void FinishFire()
    {
        isFiring = false; // Marcar el arma como lista para disparar nuevamente
    }

    public virtual void Walk()
    {
    }

    public virtual void Idle()
    {
    }

    public int GetAmmo()
    {
        return ammo; // Devolver la cantidad de munición actual
    }

    public virtual void setCanAuto()
    {
    }

    public bool getCanAuto()
    {
        return canAuto; // Devolver el estado de canAuto
    }

    public virtual void StopFiringAnim()
    {
        
    }

}
