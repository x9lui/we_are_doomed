using UnityEngine;
using System.Collections;

public class Pistol2 : Gun
{
    private bool hasAppliedDamage = false;

    public override void Fire()
    {
        if (isFiring)
        {
            Debug.Log("Pistol2 is already firing!");
            return;
        }

        if (ammo <= 0)
        {
            Debug.Log("Out of ammo!");
            spriteAnim.SetBool("Fire", false); // O el parámetro que uses
            isFiring = false; // <- IMPORTANTE
            return;
        }

        isFiring = true; // Marcar el arma como disparando
        ammo--; // Reducir la munición
        Debug.Log($"Pistol2 fired! Ammo left: {ammo}");
        spriteAnim.SetTrigger("Fire"); // Activar la animación de disparo
        hasAppliedDamage = false; // Resetear para este disparo

        StartCoroutine(WaitForDamageSprite());
        // Finalizar el disparo después de un breve retraso
        StartCoroutine(FinishFireAfterDelay(0.1f, 0.2f));
    }

    private IEnumerator WaitForDamageSprite()
    {
        // Esperar hasta que el sprite sea 'shoot_2'
        while (!hasAppliedDamage)
        {
            if (gunImage != null && gunImage.sprite.name == "shoot_2")
            {
                // Aquí se hace el raycast y el daño
                HandleRaycastAndDamage();
                hasAppliedDamage = true;
            }

            yield return null; // Esperar al siguiente frame
        }
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
        //Debug.Log("Pistol2: Walking");
    }

    public override void Idle()
    {
        spriteAnim.SetBool("isWalking", false);
        //Debug.Log("Pistol2: Idle");
    }

    public override void setCanAuto()
    {
        canAuto = false;
    }
}