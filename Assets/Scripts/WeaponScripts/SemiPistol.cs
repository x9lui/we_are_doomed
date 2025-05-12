using UnityEngine;

public class SemiPistol : Gun
{


    public override void Fire()
    {
        if (ammo <= 0)
        {
            Debug.Log("SemiPistol: Out of ammo!");
            return;
        }

        ammo--; // Reducir la munición
        Debug.Log($"SemiPistol fired! Ammo left: {ammo}");
        spriteAnim.SetTrigger("Fire"); // Activar la animación de disparo

        // Usar el método genérico para manejar el raycast y el daño
        HandleRaycastAndDamage();
    }

    public override void Walk()
    {
        spriteAnim.SetBool("isWalking", true); // Set the walking animation
        // Implementar la lógica de caminar con el puño
        Debug.Log("Fist: Walking");
    }

    public override void Idle()
    {
        spriteAnim.SetBool("isWalking", false); // Set the walking animation

        // Implementar la lógica de estar inactivo con el puño
        Debug.Log("Fist: Idle");
    }   
}