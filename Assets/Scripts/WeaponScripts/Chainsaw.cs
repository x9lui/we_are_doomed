using UnityEngine;
using System.Collections;

public class Chainsaw : Gun
{

    public override void Fire()
    {
        if (Input.GetMouseButton(0) && Time.time >= nextTimeToFire)
        {
            if (ammo <= 0)
            {
                Debug.Log("Out of ammo!");
                spriteAnim.SetBool("Fire", false); // O el parámetro que uses
                isFiring = false; // <- IMPORTANTE
                return;
            }

            nextTimeToFire = Time.time + fireRate;
            spriteAnim.SetBool("Fire", true);

            // Solo hacer daño y consumir munición si la animación está en el estado "Fire"
            AnimatorStateInfo stateInfo = spriteAnim.GetCurrentAnimatorStateInfo(0);

            if (stateInfo.IsName("Chainsaw Fire")) // Cambia "Fire" por el nombre exacto de tu animación
            {
                if (!audioSource.isPlaying)
                {
                    audioSource.PlayOneShot(GunSHot);
                }
                ammo--;
                Debug.Log($"Chainsaw fired! Ammo left: {ammo}");
                HandleRaycastAndDamage();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            spriteAnim.SetBool("Fire", false);
        }
    }

    public override void Walk()
    {
        spriteAnim.SetBool("isWalking", true);
        //Debug.Log("Chainsaw: Walking");
    }

    public override void Idle()
    {
        spriteAnim.SetBool("isWalking", false);
        //Debug.Log("Chainsaw: Idle");
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