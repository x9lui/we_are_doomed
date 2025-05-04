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

    void Update()
    {
        if (playerMove == null || playerMove.currentGun == null)
        {
            Debug.LogWarning("No active weapon or PlayerMove script is missing!");
            return;
        }

        // Obtener el valor de RayCastDistance del arma actual
        float rayCastDistance = playerMove.currentGun.RayCastDistance;

        // Crear un rayo desde la posición del jugador en la dirección hacia adelante
        Ray ray = new Ray(transform.position, transform.forward);

        // Dibujar el rayo en la vista de escena para depuración
        Debug.DrawRay(ray.origin, ray.direction * rayCastDistance, Color.red);

        // Realizar el raycast y verificar si golpea un objeto dentro de la distancia especificada
        if (Physics.Raycast(ray, out RaycastHit hit, rayCastDistance)) //Habria que flitrar por enemigo
        {
            Debug.Log("Hit: " + hit.collider.gameObject.name); // Registrar el nombre del objeto golpeado
            // Aquí puedes agregar lógica adicional para interactuar con el objeto golpeado
        }
    }
}
