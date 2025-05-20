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
                isFiring = false;
                return;
            }

            nextTimeToFire = Time.time + fireRate;
            spriteAnim.SetBool("Fire", true);

            AnimatorStateInfo stateInfo = spriteAnim.GetCurrentAnimatorStateInfo(0);

            if (stateInfo.IsName("Chainsaw Fire")) 
            {
                if (!audioSource.isPlaying)
                {
                    AudioManager.Instance.ReproducirEfectos(GunSHot);
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