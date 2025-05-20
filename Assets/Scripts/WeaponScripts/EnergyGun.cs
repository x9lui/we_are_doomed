using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnergyGun : Gun
{
    public int pellets = 40;
    public float spreadAngle = 30f;
    private bool hasAppliedDamage = false;

    public override void Fire()
    {
        if (isFiring)
        {
            Debug.Log("EnergyGun is already firing!");
            return;
        }

        if (ammo <= 0)
        {
            Debug.Log("Out of ammo!");
            spriteAnim.SetBool("Fire", false); // O el parámetro que uses
            isFiring = false;
            return;
        }

        isFiring = true;
        ammo--;
        AudioManager.Instance.ReproducirEfectos(GunSHot);

        Debug.Log($"EnergyGun fired! Ammo left: {ammo}");
        spriteAnim.SetTrigger("Fire");
        hasAppliedDamage = false;

        StartCoroutine(WaitForDamageSprite());
        StartCoroutine(FinishFireAfterDelay(0.2f, 0.5f));
    }

    private IEnumerator WaitForDamageSprite()
    {
        // Esperar hasta que el sprite sea 'shoot_2'
        while (!hasAppliedDamage)
        {

            if (gunImage != null && gunImage.sprite.name == "shoot_2")
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
        Debug.Log("EnergyGun: Action completed.");

        yield return new WaitForSeconds(unlockDelay);
        FinishFire();
        Debug.Log("EnergyGun: Ready to fire again.");
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