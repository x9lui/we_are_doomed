using UnityEngine;
using System.Collections.Generic;
using System;

public class InventoryScript : MonoBehaviour
{
    public WeaponHUDDisplay weaponHUDDisplay;

    public enum WeaponType
    {
        Fist,
        Pistol,
        Shotgun,
        Rifle,
        RocketLauncher,
        Melee
    }

    [System.Serializable]
    public class WeaponSlot
    {
        public WeaponType weaponType; // Tipo de arma para este slot
        public string weaponName; // Nombre del arma específica (por ejemplo, "Pistola Automática")
    }

    [SerializeField] private List<WeaponSlot> inventory = new List<WeaponSlot>(7);

    private List<WeaponSlot> Originalinventory;

    public void Start()
    {
        // Inicializar el inventario con slots vacíos
        foreach (WeaponType type in System.Enum.GetValues(typeof(WeaponType)))
        {
            inventory.Add(new WeaponSlot { weaponType = type, weaponName = null });
        }

        // Copia del Inventario Original para cuando reaparezcamos
        Originalinventory = new List<WeaponSlot>();
        foreach (var slot in inventory)
        {
            Originalinventory.Add(new WeaponSlot
            {
                weaponType = slot.weaponType,
                weaponName = slot.weaponName
            });
        }
        weaponHUDDisplay?.UpdateHUD(inventory);
    }

    public bool AddWeapon(WeaponType type, string weaponName)
    {
        // Buscar el slot correspondiente al tipo de arma
        WeaponSlot slot = inventory.Find(s => s.weaponType == type);

        if (slot != null && string.IsNullOrEmpty(slot.weaponName))
        {
            slot.weaponName = weaponName; // Añadir el arma al slot
            Debug.Log($"Added {weaponName} to {type} slot.");

            weaponHUDDisplay?.ActivateWeaponUI(type, weaponName);

            return true;
        }

        Debug.LogWarning($"Slot for {type} is already occupied or does not exist.");
        return false;
    }

    public bool RemoveWeapon(WeaponType type)
    {
        // Buscar el slot correspondiente al tipo de arma
        WeaponSlot slot = inventory.Find(s => s.weaponType == type);

        if (slot != null && !string.IsNullOrEmpty(slot.weaponName))
        {
            Debug.Log($"Removed {slot.weaponName} from {type} slot.");
            slot.weaponName = null; // Vaciar el slot

            weaponHUDDisplay?.DeactivateWeaponUI(type);

            return true;
        }

        Debug.LogWarning($"Slot for {type} is already empty or does not exist.");
        return false;
    }

    public string GetWeapon(WeaponType type)
    {
        // Obtener el arma específica en el slot del tipo de arma
        WeaponSlot slot = inventory.Find(s => s.weaponType == type);
        if (slot != null && !string.IsNullOrEmpty(slot.weaponName))
        {
            weaponHUDDisplay?.HighlightCurrentWeapon(type, slot.weaponName, inventory);
            return slot.weaponName;
        }

        return null;
    }

    public void ResetInventory()
    {
        inventory = new List<WeaponSlot>();
        foreach (var slot in Originalinventory)
        {
            inventory.Add(new WeaponSlot
            {
                weaponType = slot.weaponType,
                weaponName = slot.weaponName
            });
        }
        weaponHUDDisplay?.UpdateHUD(inventory);
    }
}
