using UnityEngine;

public class ShotgunNormal : Gun
{
    public int pellets = 8; // Número de perdigones disparados por la escopeta
    public float spreadAngle = 10f; // Ángulo de dispersión de los perdigones

    public override void Fire()
    {
        if (ammo <= 0)
        {
            Debug.Log("ShotgunNormal: Out of ammo!");
            return;
        }

        ammo--; // Reducir la munición
        Debug.Log($"ShotgunNormal fired! Ammo left: {ammo}");
        spriteAnim.SetTrigger("Fire"); // Activar la animación de disparo

        // Disparar múltiples perdigones
        for (int i = 0; i < pellets; i++)
        {
            HandleRaycastAndDamage(); // Usar el método genérico para cada perdigón
        }
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