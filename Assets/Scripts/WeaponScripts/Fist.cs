using UnityEngine;

public class Fist : Gun
{
    public float punchRange = 2f; // Alcance del golpe

    public override void Fire()
    {
        Debug.Log("Fist: Punch!");

        // // Realizar un raycast para detectar impactos
        // RaycastHit hit;
        // if (Physics.Raycast(fpsCamera.transform.position, fpsCamera.transform.forward, out hit, punchRange))
        // {
        //     Debug.Log($"Fist hit: {hit.collider.name}");

        //     // Aplicar daño si el objeto impactado tiene un componente de salud
        //     var target = hit.collider.GetComponent<Health>();
        //     if (target != null)
        //     {
        //         target.TakeDamage(damage);
        //     }
        // }
    }
}