using UnityEngine;

public class Fist : Gun
{
    public float punchRange = 2f; // Alcance del golpe

    public override void Fire()
    {
        Debug.Log("Fist: Punch!");
        spriteAnim.SetTrigger("Fire"); // Activar la animación de golpe

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