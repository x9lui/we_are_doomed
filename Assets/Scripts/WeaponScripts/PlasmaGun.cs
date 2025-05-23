using UnityEngine;
using System.Collections;

public class PlasmaGun : Gun
{
    public GameObject plasmaProjectilePrefab;
    public Transform firePoint; // Punto de salida del proyectil

    [System.Obsolete]
    void Awake()
    {
        if (firePoint == null)
        {
            RayCast rayCast = FindObjectOfType<RayCast>();
            if (rayCast != null)
                firePoint = rayCast.transform;
            else
                Debug.LogWarning("No RayCast script found in the scene for PlasmaGun firePoint!");
        }
    }

    public override void Fire()
    {
        if (Input.GetMouseButton(0) && Time.time >= nextTimeToFire)
        {
            if (ammo <= 0)
            {
                Debug.Log("Out of ammo!");
                spriteAnim.SetBool("Fire", false);
                isFiring = false;
                return;
            }

            nextTimeToFire = Time.time + fireRate;
            ammo--;
            Debug.Log($"PlasmaGun fired! Ammo left: {ammo}");
            spriteAnim.SetBool("Fire", true);

            if (plasmaProjectilePrefab != null && firePoint != null)
            {
                float spawnOffset = 0.5f; // Ajusta si es necesario
                AudioManager.Instance.ReproducirEfectos(GunSHot);

                Vector3 spawnPos = firePoint.position + firePoint.forward * spawnOffset;
                GameObject plasma = Instantiate(plasmaProjectilePrefab, spawnPos, firePoint.rotation);

                Collider plasmaCol = plasma.GetComponent<Collider>();
                GameObject player = GameObject.FindWithTag("Player");
                if (player != null && plasmaCol != null)
                {
                    Collider playerCol = player.GetComponent<Collider>();
                    if (playerCol != null)
                        Physics.IgnoreCollision(plasmaCol, playerCol);
                }
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