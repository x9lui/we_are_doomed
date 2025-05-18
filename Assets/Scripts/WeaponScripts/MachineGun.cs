using UnityEngine;
using System.Collections;

public class MachineGun : Gun
{

    public override void Fire()
    {
        if (Input.GetMouseButton(0) && Time.time >= nextTimeToFire)
        {
            if (ammo <= 0)
            {
                Debug.Log("Out of ammo!");
                spriteAnim.SetBool("Fire", false);
                isFiring = false;
                return;
            }

            nextTimeToFire = Time.time + fireRate;
            ammo--;
            Debug.Log($"MachineGun fired! Ammo left: {ammo}");
            spriteAnim.SetBool("Fire", true);

            audioSource.PlayOneShot(GunSHot);

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
    }

    public override void Idle()
    {
        spriteAnim.SetBool("isWalking", false);
    }

    public override void setCanAuto()
    {
        canAuto = true;
    }

    public override void StopFiringAnim()
    {
            spriteAnim.SetBool("Fire", false);
    }
}