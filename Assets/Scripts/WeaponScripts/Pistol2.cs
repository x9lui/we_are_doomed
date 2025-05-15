using UnityEngine;
using System.Collections;

public class Pistol2 : Gun
{
    public override void Fire()
    {
        if (isFiring)
        {
            Debug.Log("Pistol2 is already firing!");
            return;
        }

        if (ammo <= 0)
        {
            Debug.Log("Pistol2: Out of ammo!");
            return;
        }

        isFiring = true; // Marcar el arma como disparando
        ammo--; // Reducir la munición
        Debug.Log($"Pistol2 fired! Ammo left: {ammo}");
        spriteAnim.SetTrigger("Fire"); // Activar la animación de disparo

        // Llamar al método genérico para manejar el raycast y el daño
        HandleRaycastAndDamage();

        // Finalizar el disparo después de un breve retraso
        StartCoroutine(FinishFireAfterDelay(0.1f, 0.2f));
    }

    private IEnumerator FinishFireAfterDelay(float actionDelay, float unlockDelay)
    {
        yield return new WaitForSeconds(actionDelay); // Esperar a que termine la acción principal
        Debug.Log("Pistol2: Action completed.");

        yield return new WaitForSeconds(unlockDelay); // Esperar tiempo adicional antes de desbloquear
        FinishFire(); // Marcar el arma como lista para disparar nuevamente
        Debug.Log("Pistol2: Ready to fire again.");
    }

    public override void Walk()
    {
        spriteAnim.SetBool("isWalking", true);
        Debug.Log("Pistol2: Walking");
    }

    public override void Idle()
    {
        spriteAnim.SetBool("isWalking", false);
        Debug.Log("Pistol2: Idle");
    }

    public override void setCanAuto()
    {
        canAuto = false;
    }
}