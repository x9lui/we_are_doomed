using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SemiPistol : Gun
{
    private bool hasAppliedDamage = false;

    public override void Fire()
    {
        if (isFiring)
        {
            Debug.Log("SemiPistol is already firing!");
            return;
        }

        if (ammo <= 0)
        {
            Debug.Log("Out of ammo!");
            spriteAnim.SetBool("Fire", false); // O el parámetro que uses
            isFiring = false; // <- IMPORTANTE
            return;
        }

        isFiring = true;
        ammo--;
        Debug.Log($"SemiPistol fired! Ammo left: {ammo}");
        spriteAnim.SetTrigger("Fire");
        hasAppliedDamage = false; // Resetear para este disparo

        StartCoroutine(WaitForDamageSprite());
        StartCoroutine(FinishFireAfterDelay(0.1f, 0.2f));
    }

    private IEnumerator WaitForDamageSprite()
    {
        // Esperar hasta que el sprite sea 'shoot_2'
        while (!hasAppliedDamage)
        {
            if (gunImage != null && gunImage.sprite.name == "shoot_2")
            {
        audioSource.PlayOneShot(GunSHot);

                // Aquí se hace el raycast y el daño
                HandleRaycastAndDamage();
                hasAppliedDamage = true;
            }

            yield return null; // Esperar al siguiente frame
        }
    }

    private IEnumerator FinishFireAfterDelay(float actionDelay, float unlockDelay)
    {
        yield return new WaitForSeconds(actionDelay);
        yield return new WaitForSeconds(unlockDelay);
        FinishFire();
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
        canAuto = false;
    }
}