using UnityEngine;

public class RayCast : MonoBehaviour
{
    private PlayerMove playerMove; // Referencia al script PlayerMove para obtener el arma actual

    [System.Obsolete]
    void Start()
    {
        // Obtener la referencia al script PlayerMove en el objeto del jugador
        playerMove = FindObjectOfType<PlayerMove>();
        if (playerMove == null)
        {
            Debug.LogError("PlayerMove script not found!");
        }
    }

    public GameObject CheckRayCastCollisionWithEnemy()
    {
        if (playerMove == null || playerMove.currentGun == null)
        {
            Debug.LogWarning("No active weapon or PlayerMove script is missing!");
            return null;
        }

        // Obtener el valor de RayCastDistance del arma actual
        float rayCastDistance = playerMove.currentGun.RayCastDistance;

        // Crear un rayo desde la posición del jugador en la dirección hacia adelante
        Ray ray = new Ray(transform.position, transform.forward);

        // Dibujar el rayo en la vista de escena para depuración
        Debug.DrawRay(ray.origin, ray.direction * rayCastDistance, Color.red);

        // Realizar el raycast y verificar si golpea un objeto dentro de la distancia especificada
        if (Physics.Raycast(ray, out RaycastHit hit, rayCastDistance))
        {
            // Comprobar si el objeto golpeado tiene la etiqueta "Enemy"
            if (hit.collider.CompareTag("Enemy"))
            {
                Debug.Log($"Hit enemy: {hit.collider.gameObject.name}");
                return hit.collider.gameObject; // Devolver el objeto enemigo golpeado
            }
        }

        // Si no se golpeó a ningún enemigo, devolver null
        return null;
    }
}
