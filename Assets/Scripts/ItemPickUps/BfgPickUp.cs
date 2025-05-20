using UnityEngine;

public class BfgPickUp : ItemPickUp
{
    public string weaponName = "Bfg"; // Nombre del arma

    protected override void OnPickUp(Collider player)
    {
        if (playerInventory != null)
        {
            if (playerInventory.AddWeapon(InventoryScript.WeaponType.RocketLauncher, weaponName))
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
