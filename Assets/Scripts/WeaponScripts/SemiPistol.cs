using UnityEngine;

public class SemiPistol : Gun
{
    public override void Fire()
    {
        if (ammo <= 0)
        {
            Debug.Log("SemiPistol: Out of ammo!");
            return;
        }

        ammo--; // Reducir la munición
        Debug.Log($"SemiPistol fired! Ammo left: {ammo}");

        // // Efectos visuales y de sonido
        // if (muzzleFlash != null) muzzleFlash.Play();
        // if (gunSound != null) gunSound.Play();

        // // Realizar un raycast para detectar impactos
        // RaycastHit hit;
        // if (Physics.Raycast(fpsCamera.transform.position, fpsCamera.transform.forward, out hit, range))
        // {
        //     Debug.Log($"SemiPistol hit: {hit.collider.name}");

        //     // Aplicar daño si el objeto impactado tiene un componente de salud
        //     var target = hit.collider.GetComponent<Health>();
        //     if (target != null)
        //     {
        //         target.TakeDamage(damage);
        //     }
        // }
    }
}