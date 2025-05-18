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
            spriteAnim.SetBool("Fire", false);
            isFiring = false;
            return;
        }

        isFiring = true;
        ammo--;
        Debug.Log($"Pistol2 fired! Ammo left: {ammo}");
        spriteAnim.SetTrigger("Fire");
        hasAppliedDamage = false;

        StartCoroutine(WaitForDamageSprite());
        StartCoroutine(FinishFireAfterDelay(0.1f, 0.2f));
    }

    private IEnumerator WaitForDamageSprite()
    {
        while (!hasAppliedDamage)
        {
            if (gunImage != null && gunImage.sprite.name == "shoot_2")
            {
                AudioManager.Instance.ReproducirEfectos(GunSHot);

                HandleRaycastAndDamage();
                hasAppliedDamage = true;
            }

            yield return null;
        }
    }


    private IEnumerator FinishFireAfterDelay(float actionDelay, float unlockDelay)
    {
        yield return new WaitForSeconds(actionDelay);
        Debug.Log("Pistol2: Action completed.");

        yield return new WaitForSeconds(unlockDelay);
        FinishFire();
        Debug.Log("Pistol2: Ready to fire again.");
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