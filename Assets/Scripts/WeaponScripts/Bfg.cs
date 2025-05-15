using UnityEngine;
using System.Collections;

public class Bfg : Gun
{
    public GameObject rocketPrefab;
    public Transform firePoint; // El punto desde donde sale el cohete

    [System.Obsolete]
    void Awake()
    {
        // Si firePoint no está asignado, buscar el RayCast y usar su transform
        if (firePoint == null)
        {
            RayCast rayCast = FindObjectOfType<RayCast>();
            if (rayCast != null)
                firePoint = rayCast.transform;
            else
                Debug.LogWarning("No RayCast script found in the scene for Bfg firePoint!");
        }
    }

    public override void Fire()
    {
        if (isFiring)
        {
            Debug.Log("Bfg is already firing!");
            return;
        }

        if (ammo <= 0)
        {
            Debug.Log("Bfg: Out of ammo!");
            return;
        }

        isFiring = true;
        ammo--;
        Debug.Log($"Bfg fired! Ammo left: {ammo}");
        spriteAnim.SetTrigger("Fire");

        // Instanciar el proyectil
        if (rocketPrefab != null && firePoint != null)
        {
            float spawnOffset = 1.0f; // Ajusta este valor según el tamaño de tu arma/cohete
            Vector3 spawnPos = firePoint.position + firePoint.forward * spawnOffset;
            GameObject rocket = Instantiate(rocketPrefab, spawnPos, firePoint.rotation);

            // Ignorar colisión con el jugador
            Collider rocketCol = rocket.GetComponent<Collider>();
            GameObject player = GameObject.FindWithTag("Player"); // Asegúrate de que el jugador tenga el tag "Player"
            if (player != null && rocketCol != null)
            {
                Collider playerCol = player.GetComponent<Collider>();
                if (playerCol != null)
                    Physics.IgnoreCollision(rocketCol, playerCol);
            }
        }

        StartCoroutine(FinishFireAfterDelay(0.1f, 0.2f));
    }

    private IEnumerator FinishFireAfterDelay(float actionDelay, float unlockDelay)
    {
        yield return new WaitForSeconds(actionDelay); // Esperar a que termine la acción principal
        //Debug.Log("Bfg: Action completed.");

        yield return new WaitForSeconds(unlockDelay); // Esperar tiempo adicional antes de desbloquear
        FinishFire(); // Marcar el arma como lista para disparar nuevamente
        //Debug.Log("Bfg: Ready to fire again.");
    }

    public override void Walk()
    {
        spriteAnim.SetBool("isWalking", true);
        //Debug.Log("Bfg: Walking");
    }

    public override void Idle()
    {
        spriteAnim.SetBool("isWalking", false);
        //Debug.Log("Bfg: Idle");
    }

    public override void setCanAuto()
    {
        canAuto = false;
    }
}