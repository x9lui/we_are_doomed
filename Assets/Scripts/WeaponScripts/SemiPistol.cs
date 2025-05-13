using UnityEngine;
using System.Collections;

public class SemiPistol : Gun
{
    public override void Fire()
    {
        if (isFiring)
        {
            Debug.Log("SemiPistol is already firing!");
            return;
        }

        if (ammo <= 0)
        {
            Debug.Log("SemiPistol: Out of ammo!");
            return;
        }

        isFiring = true; // Marcar el arma como disparando
        ammo--; // Reducir la munición
        Debug.Log($"SemiPistol fired! Ammo left: {ammo}");
        spriteAnim.SetTrigger("Fire"); // Activar la animación de disparo

        // Llamar al método genérico para manejar el raycast y el daño
        HandleRaycastAndDamage();

        // Finalizar el disparo después de un breve retraso
        StartCoroutine(FinishFireAfterDelay(0.1f, 0.2f));
    }

    private IEnumerator FinishFireAfterDelay(float actionDelay, float unlockDelay)
    {
        yield return new WaitForSeconds(actionDelay); // Esperar a que termine la acción principal
        Debug.Log("SemiPistol: Action completed.");

        yield return new WaitForSeconds(unlockDelay); // Esperar tiempo adicional antes de desbloquear
        FinishFire(); // Marcar el arma como lista para disparar nuevamente
        Debug.Log("SemiPistol: Ready to fire again.");
    }

    public override void Walk()
    {
        spriteAnim.SetBool("isWalking", true);
        Debug.Log("SemiPistol: Walking");
    }

    public override void Idle()
    {
        spriteAnim.SetBool("isWalking", false);
        Debug.Log("SemiPistol: Idle");
    }
}