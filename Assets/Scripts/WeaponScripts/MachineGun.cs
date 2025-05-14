using UnityEngine;
using System.Collections;

public class MachineGun : Gun
{
    private float nextTimeToFire = 0f; // Tiempo hasta el próximo disparo permitido

    public override void Fire()
    {
        // Verificar si el botón izquierdo del ratón está presionado y si se puede disparar
        if (Input.GetMouseButton(0) && Time.time >= nextTimeToFire)
        {
            if (ammo <= 0)
            {
                Debug.Log("MachineGun: Out of ammo!");
                spriteAnim.SetBool("Fire", false); // Desactivar la animación de disparo
                return;
            }

            nextTimeToFire = Time.time + fireRate; // Configurar el tiempo para el próximo disparo
            ammo--; // Reducir la munición
            Debug.Log($"MachineGun fired! Ammo left: {ammo}");
            spriteAnim.SetBool("Fire", true); // Activar la animación de disparo

            // Manejar el disparo y el daño
            HandleRaycastAndDamage();
        }

        // Desactivar la animación de disparo si el botón se suelta
        if (Input.GetMouseButtonUp(0))
        {
            spriteAnim.SetBool("Fire", false);
        }
    }

    public override void Walk()
    {
        spriteAnim.SetBool("isWalking", true);
        Debug.Log("MachineGun: Walking");
    }

    public override void Idle()
    {
        spriteAnim.SetBool("isWalking", false);
        Debug.Log("MachineGun: Idle");
    }

    public override void setCanAuto()
    {
        canAuto = true; // Configurar el arma como automática
    }

    public override void StopFiringAnim()
    {
            spriteAnim.SetBool("Fire", false);
    }
}