using UnityEngine;

public class RayCast : MonoBehaviour
{
    private PlayerMove playerMove; // Referencia al script PlayerMove para obtener el arma actual
    private RaycastHit hit; // Almacena la información del impacto del raycast
    private bool hasHitEnemy; // Indica si el raycast ha golpeado a un enemigo

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

    void FixedUpdate()
    {
        UpdateRaycast(); // Actualizar el raycast en FixedUpdate
    }

    private void UpdateRaycast()
    {
        if (playerMove == null || playerMove.currentGun == null)
        {
            Debug.LogWarning("No active weapon or PlayerMove script is missing!");
            hasHitEnemy = false;
            return;
        }

        // Obtener el valor de RayCastDistance del arma actual
        float rayCastDistance = playerMove.currentGun.RayCastDistance;

        // Ajustar el origen del rayo para que esté un poco más adelante del jugador
        Vector3 rayOrigin = transform.position + transform.forward * 0.5f; // Desplazar el origen 0.5 unidades hacia adelante

        // Crear un rayo desde la posición ajustada en la dirección hacia adelante
        Ray ray = new Ray(rayOrigin, transform.forward);

        // Dibujar el rayo en la vista de escena para depuración
        Debug.DrawRay(ray.origin, ray.direction * rayCastDistance, Color.red);

        // Realizar el raycast y almacenar el resultado
        hasHitEnemy = Physics.Raycast(ray, out hit, rayCastDistance);
    }

    public GameObject CheckRayCastCollisionWithEnemy()
    {
        if (hasHitEnemy && hit.collider.CompareTag("Enemy"))
        {
            Debug.Log($"Hit enemy: {hit.collider.gameObject.name}");
            return hit.collider.gameObject; // Devolver el objeto enemigo golpeado
        }

        // Si no se golpeó a ningún enemigo, devolver null
        return null;
    }
}
