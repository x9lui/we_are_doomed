using UnityEngine;

public class SemiPistolPickUp : ItemPickUp
{
    public string weaponName = "SemiPistol"; // Nombre del arma

    protected override void OnPickUp(Collider player)
    {
        // Verificar si el jugador tiene un inventario
        if (playerInventory != null)
        {
            // Intentar añadir el arma al inventario del jugador
            if (playerInventory.AddWeapon(InventoryScript.WeaponType.Pistol, weaponName))
            {
                Debug.Log($"Picked up {weaponName}.");
                // Aquí puedes añadir la lógica para otorgar munición al arma
                // playerInventory.AddAmmo(weaponName, ammoAmount);
                ActivateWeapon(player, weaponName); // Activar el arma en el jugador
                Destroy(gameObject); // Destruir el objeto después de recogerlo

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
        // Aquí puedes implementar la lógica para activar el arma en el jugador

    }
}
