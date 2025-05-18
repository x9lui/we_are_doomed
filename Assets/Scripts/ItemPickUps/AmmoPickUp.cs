using UnityEngine;

public class AmmoPickUp : ItemPickUp
{
    public int ammoAmount = 10; // Cantidad de munición que otorga

    protected override void OnPickUp(Collider player)
    {
        playerInventory.AddAmmo(ammoAmount);
            Destroy(gameObject);

    }
}
