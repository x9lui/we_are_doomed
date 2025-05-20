using UnityEngine;

public class EnergyGunlPickUp : ItemPickUp
{
    public string weaponName = "EnergyGun"; // Nombre del arma

    protected override void OnPickUp(Collider player)
    {
        if (playerInventory != null)
        {
            if (playerInventory.AddWeapon(InventoryScript.WeaponType.Shotgun, weaponName))
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
