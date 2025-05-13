using UnityEngine;
using System.Collections;

public class ShotgunNormal : Gun
{
    public int pellets = 8; // Número de perdigones disparados por la escopeta
    public float spreadAngle = 10f; // Ángulo de dispersión de los perdigones

    public override void Fire()
    {
        if (isFiring)
        {
            Debug.Log("ShotgunNormal is already firing!");
            return;
        }

        if (ammo <= 0)
        {
            Debug.Log("ShotgunNormal: Out of ammo!");
            return;
        }

        isFiring = true; // Marcar el arma como disparando
        ammo--; // Reducir la munición
        Debug.Log($"ShotgunNormal fired! Ammo left: {ammo}");
        spriteAnim.SetTrigger("Fire"); // Activar la animación de disparo

        // Disparar múltiples perdigones
        for (int i = 0; i < pellets; i++)
        {
            HandleRaycastAndDamage(); // Usar el método genérico para cada perdigón
        }

        // Finalizar el disparo después de un breve retraso
        StartCoroutine(FinishFireAfterDelay(0.2f, 0.5f));
    }

    private IEnumerator FinishFireAfterDelay(float actionDelay, float unlockDelay)
    {
        yield return new WaitForSeconds(actionDelay); // Esperar a que termine la acción principal
        Debug.Log("ShotgunNormal: Action completed.");

        yield return new WaitForSeconds(unlockDelay); // Esperar tiempo adicional antes de desbloquear
        FinishFire(); // Marcar el arma como lista para disparar nuevamente
        Debug.Log("ShotgunNormal: Ready to fire again.");
    }

    public override void Walk()
    {
        spriteAnim.SetBool("isWalking", true);
        Debug.Log("ShotgunNormal: Walking");
    }

    public override void Idle()
    {
        spriteAnim.SetBool("isWalking", false);
        Debug.Log("ShotgunNormal: Idle");
    }
}