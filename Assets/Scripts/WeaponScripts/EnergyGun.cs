using UnityEngine;
using System.Collections;

public class EnergyGun : Gun
{
    public int pellets = 40; // Número de perdigones disparados por la escopeta
    public float spreadAngle = 30f; // Ángulo de dispersión de los perdigones

    
    public override void Fire()
    {
        if (isFiring)
        {
            Debug.Log("EnergyGun is already firing!");
            return;
        }

        if (ammo <= 0)
        {
            Debug.Log("EnergyGun: Out of ammo!");
            return;
        }

        isFiring = true; // Marcar el arma como disparando
        ammo--; // Reducir la munición
        Debug.Log($"EnergyGun fired! Ammo left: {ammo}");
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
        Debug.Log("EnergyGun: Action completed.");

        yield return new WaitForSeconds(unlockDelay); // Esperar tiempo adicional antes de desbloquear
        FinishFire(); // Marcar el arma como lista para disparar nuevamente
        Debug.Log("EnergyGun: Ready to fire again.");
    }

    public override void Walk()
    {
        spriteAnim.SetBool("isWalking", true);
        //Debug.Log("EnergyGun: Walking");
    }

    public override void Idle()
    {
        spriteAnim.SetBool("isWalking", false);
        //Debug.Log("EnergyGun: Idle");
    }

    public override void setCanAuto()
    {
        canAuto = false;
    }

}