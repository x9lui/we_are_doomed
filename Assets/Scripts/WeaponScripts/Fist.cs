using UnityEngine;
using System.Collections;

public class Fist : Gun
{
    public float punchRange = 2f; // Alcance del golpe

    private IEnumerator PerformPunchAfterDelay(float actionDelay, float unlockDelay)
    {
        isFiring = true; // Marcar el arma como disparando
        yield return new WaitForSeconds(actionDelay); // Esperar el tiempo de la acción principal

        // Usar el método genérico para manejar el raycast y el daño
        HandleRaycastAndDamage();

        yield return new WaitForSeconds(unlockDelay); // Esperar tiempo adicional antes de desbloquear
        FinishFire(); // Marcar el arma como lista para disparar nuevamente
        Debug.Log("Fist: Ready to punch again.");
    }

    public override void Fire()
    {
        if (isFiring)
        {
            Debug.Log("Fist is already punching!");
            return;
        }

        audioSource.PlayOneShot(sound);
        Debug.Log("Fist: Punch!");
        spriteAnim.SetTrigger("Fire"); // Activar la animación de golpe

        // Iniciar la corutina con retrasos específicos
        StartCoroutine(PerformPunchAfterDelay(0.25f, 0.2f));
    }

    public override void Walk()
    {
        spriteAnim.SetBool("isWalking", true); // Set the walking animation
        //Debug.Log("Fist: Walking");
    }

    public override void Idle()
    {
        spriteAnim.SetBool("isWalking", false); // Set the walking animation
        //Debug.Log("Fist: Idle");
    }

    public override void setCanAuto()
    {
        canAuto = false;
    }
}