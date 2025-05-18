using UnityEngine;

public class PlasmaGunPickUp : ItemPickUp
{
    public string weaponName = "PlasmaGun"; // Nombre del arma

    protected override void OnPickUp(Collider player)
    {
        if (playerInventory != null)
        {
            if (playerInventory.AddWeapon(InventoryScript.WeaponType.Rifle, weaponName))
            {
                Debug.Log($"Picked up {weaponName}.");
                ActivateWeapon(player, weaponName);
                Destroy(gameObject);

            }
            else
            {
                Debug.LogWarning($"Failed to pick up {weaponName}.");
                
            }
        }
        else
        {
            Debug.LogError("Player does not have an InventoryScript component!");
        }
    }

    private void ActivateWeapon(Collider player, string weaponName)
    {

    }
}
