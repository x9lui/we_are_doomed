using UnityEngine;

public class ShotgunNormal : Gun
{
    public int pellets = 8; // Número de perdigones disparados por la escopeta
    public float spreadAngle = 10f; // Ángulo de dispersión de los perdigones

    public override void Fire()
    {
        if (ammo <= 0)
        {
            Debug.Log("ShotgunNormal: Out of ammo!");
            return;
        }

        ammo--; // Reducir la munición
        Debug.Log($"ShotgunNormal fired! Ammo left: {ammo}");

    //     // Efectos visuales y de sonido
    //     if (muzzleFlash != null) muzzleFlash.Play();
    //     if (gunSound != null) gunSound.Play();

    //     // Disparar múltiples perdigones
    //     for (int i = 0; i < pellets; i++)
    //     {
    //         Vector3 shootDirection = fpsCamera.transform.forward;
    //         shootDirection.x += Random.Range(-spreadAngle, spreadAngle) * 0.01f;
    //         shootDirection.y += Random.Range(-spreadAngle, spreadAngle) * 0.01f;

    //         RaycastHit hit;
    //         if (Physics.Raycast(fpsCamera.transform.position, shootDirection, out hit, range))
    //         {
    //             Debug.Log($"ShotgunNormal hit: {hit.collider.name}");

    //             // Aplicar daño si el objeto impactado tiene un componente de salud
    //             var target = hit.collider.GetComponent<Health>();
    //             if (target != null)
    //             {
    //                 target.TakeDamage(damage);
    //             }
    //         }
    //     }
    }
}