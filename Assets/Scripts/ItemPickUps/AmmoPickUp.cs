using UnityEngine;

public class AmmoPickUp : ItemPickUp
{
    public int ammoAmount = 10; // Cantidad de munición que otorga

    protected override void OnPickUp(Collider player)
    {
        playerInventory.AddAmmo(ammoAmount); // Añadir munición al inventario del jugador
            Destroy(gameObject); // Destruir el objeto después de recogerlo

    }
}
