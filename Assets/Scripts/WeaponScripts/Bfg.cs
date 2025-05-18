using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Bfg : Gun
{
    public GameObject rocketPrefab;
    public Transform firePoint; // El punto desde donde sale el cohete
    private bool hasFiredProjectile = false;

    [System.Obsolete]
    void Awake()
    {
        // Si firePoint no está asignado, buscar el RayCast y usar su transform
        if (firePoint == null)
        {
            RayCast rayCast = FindObjectOfType<RayCast>();
            if (rayCast != null)
                firePoint = rayCast.transform;
            else
                Debug.LogWarning("No RayCast script found in the scene for Bfg firePoint!");
        }
    }

    public override void Fire()
    {
        if (isFiring)
        {
            Debug.Log("Bfg is already firing!");
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
        Debug.Log($"Bfg fired! Ammo left: {ammo}");
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
                    float spawnOffset = 1.0f;
                    audioSource.PlayOneShot(GunSHot);

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