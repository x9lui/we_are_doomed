using UnityEngine;
using System.Collections;

public class RocketLauncher : Gun
{
    public GameObject rocketPrefab;
    public Transform firePoint;
    private bool hasFiredProjectile = false;

    [System.Obsolete]
    void Awake()
    {
        if (firePoint == null)
        {
            RayCast rayCast = FindObjectOfType<RayCast>();
            if (rayCast != null)
                firePoint = rayCast.transform;
            else
                Debug.LogWarning("No RayCast script found in the scene for RocketLauncher firePoint!");
        }
    }

    public override void Fire()
    {
        if (isFiring)
        {
            Debug.Log("RocketLauncher is already firing!");
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
        Debug.Log($"RocketLauncher fired! Ammo left: {ammo}");
        spriteAnim.SetTrigger("Fire");
        hasFiredProjectile = false;

        StartCoroutine(WaitForProjectileSprite());
        StartCoroutine(FinishFireAfterDelay(0.1f, 0.2f));
    }

    private IEnumerator WaitForProjectileSprite()
    {
        while (!hasFiredProjectile)
        {
            if (gunImage != null && gunImage.sprite.name == "shoot_2")
            {
                if (rocketPrefab != null && firePoint != null)
                {
                AudioManager.Instance.ReproducirEfectos(GunSHot);

                    float spawnOffset = 1.0f;
                    Vector3 spawnPos = firePoint.position + firePoint.forward * spawnOffset;
                    GameObject rocket = Instantiate(rocketPrefab, spawnPos, firePoint.rotation);

                    Collider rocketCol = rocket.GetComponent<Collider>();
                    GameObject player = GameObject.FindWithTag("Player");
                    if (player != null && rocketCol != null)
                    {
                        Collider playerCol = player.GetComponent<Collider>();
                        if (playerCol != null)
                            Physics.IgnoreCollision(rocketCol, playerCol);
                    }
                }
                hasFiredProjectile = true;
            }
            yield return null;
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