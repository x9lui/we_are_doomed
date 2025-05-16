using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShotgunNormal : Gun
{
    public int pellets = 8; // Número de perdigones disparados por la escopeta
    public float spreadAngle = 10f; // Ángulo de dispersión de los perdigones
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
            Debug.Log("ShotgunNormal: Out of ammo!");
            return;
        }

        isFiring = true;
        ammo--;
        Debug.Log($"ShotgunNormal fired! Ammo left: {ammo}");
        spriteAnim.SetTrigger("Fire");
        hasAppliedDamage = false;

        StartCoroutine(WaitForDamageSprite());
        StartCoroutine(FinishFireAfterDelay(0.2f, 0.5f));
    }

    private IEnumerator WaitForDamageSprite()
    {
        // Esperar hasta que el sprite sea 'shoot_1'
        while (!hasAppliedDamage)
        {
            if (gunImage != null && gunImage.sprite.name == "shoot_1")
            {
                // Disparar múltiples perdigones solo en este frame
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