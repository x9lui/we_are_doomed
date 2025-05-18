using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShotgunNormal : Gun
{
    public int pellets = 8;
    public float spreadAngle = 10f;
    private bool hasAppliedDamage = false;

    public override void Fire()
    {
        if (isFiring)
        {
            Debug.Log("ShotgunNormal is already firing!");
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
        AudioManager.Instance.ReproducirEfectos(GunSHot);
        Debug.Log($"ShotgunNormal fired! Ammo left: {ammo}");
        spriteAnim.SetTrigger("Fire");
        hasAppliedDamage = false;

        StartCoroutine(WaitForDamageSprite());
        StartCoroutine(FinishFireAfterDelay(0.2f, 0.5f));
    }

    private IEnumerator WaitForDamageSprite()
    {
        while (!hasAppliedDamage)
        {
            if (gunImage != null && gunImage.sprite.name == "shoot_1")
            {
                for (int i = 0; i < pellets; i++)
                {
                    HandleRaycastAndDamage();
                }
                hasAppliedDamage = true;
            }
            yield return null;
        }
    }

    private IEnumerator FinishFireAfterDelay(float actionDelay, float unlockDelay)
    {
        yield return new WaitForSeconds(actionDelay);
        Debug.Log("ShotgunNormal: Action completed.");

        yield return new WaitForSeconds(unlockDelay);
        FinishFire();
        Debug.Log("ShotgunNormal: Ready to fire again.");
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