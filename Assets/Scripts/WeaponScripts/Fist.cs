using UnityEngine;
using System.Collections;

public class Fist : Gun
{
    public float punchRange = 2f; // Alcance del golpe

    private IEnumerator PerformPunchAfterDelay(float actionDelay, float unlockDelay)
    {
        isFiring = true; // Marcar el arma como disparando
        yield return new WaitForSeconds(actionDelay);

        HandleRaycastAndDamage();

        yield return new WaitForSeconds(unlockDelay);
        FinishFire();
        Debug.Log("Fist: Ready to punch again.");
    }

    public override void Fire()
    {
        if (isFiring)
        {
            Debug.Log("Fist is already punching!");
            return;
        }

        AudioManager.Instance.ReproducirEfectos(GunSHot);
        Debug.Log("Fist: Punch!");
        spriteAnim.SetTrigger("Fire");

        StartCoroutine(PerformPunchAfterDelay(0.25f, 0.2f));
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