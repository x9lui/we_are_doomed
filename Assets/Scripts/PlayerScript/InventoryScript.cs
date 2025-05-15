using UnityEngine;
using System.Collections.Generic;

public class InventoryScript : MonoBehaviour
{
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

    void Start()
    {
        // Inicializar el inventario con slots vacíos
        foreach (WeaponType type in System.Enum.GetValues(typeof(WeaponType)))
        {
            inventory.Add(new WeaponSlot { weaponType = type, weaponName = null });
        }
    }

    public bool AddWeapon(WeaponType type, string weaponName)
    {
        // Buscar el slot correspondiente al tipo de arma
        WeaponSlot slot = inventory.Find(s => s.weaponType == type);

        if (slot != null && string.IsNullOrEmpty(slot.weaponName))
        {
            slot.weaponName = weaponName; // Añadir el arma al slot
            Debug.Log($"Added {weaponName} to {type} slot.");
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
            return true;
        }

        Debug.LogWarning($"Slot for {type} is already empty or does not exist.");
        return false;
    }

    public string GetWeapon(WeaponType type)
    {
        // Obtener el arma específica en el slot del tipo de arma
        WeaponSlot slot = inventory.Find(s => s.weaponType == type);
        return (slot != null && !string.IsNullOrEmpty(slot.weaponName)) ? slot.weaponName : null;
    }
}
