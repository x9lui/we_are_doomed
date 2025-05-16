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

    public GameObject semiPistolPickUpPrefab;
    public GameObject pistol2PickUpPrefab;
    public GameObject shotgunPickUpPrefab;
    public GameObject EnergyGunPickUpPrefab;
    public GameObject MachineGunPickUpPrefab;
    public GameObject PlasmaPickUpPrefab;
    public GameObject RocketLauncherPickUpPrefab;
    public GameObject bfgPickUpPrefab;
    public GameObject ChainSawPickUpPrefab;
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

    public bool RemoveWeapon(WeaponType type, Vector3 dropPosition)
    {
        if (type == WeaponType.Fist)
        {
            Debug.LogWarning("¡No puedes eliminar los Fist!");
            return false;
        }

        WeaponSlot slot = inventory.Find(s => s.weaponType == type);

        if (slot != null && !string.IsNullOrEmpty(slot.weaponName))
        {
            Debug.Log($"Removed {slot.weaponName} from {type} slot.");

            // Dropear el arma específica antes de vaciar el slot
            GameObject prefabToDrop = null;
            switch (slot.weaponName)
            {
                case "SemiPistol":
                    prefabToDrop = semiPistolPickUpPrefab;
                    break;
                case "Pistol2":
                    prefabToDrop = pistol2PickUpPrefab;
                    break;
                case "ShotgunNormal":
                    prefabToDrop = shotgunPickUpPrefab;
                    break;
                case "EnergyGun":
                    prefabToDrop = EnergyGunPickUpPrefab;
                    break;
                case "MachineGun":
                    prefabToDrop = MachineGunPickUpPrefab;
                    break;
                case "PlasmaGun":
                    prefabToDrop = PlasmaPickUpPrefab;
                    break;
                case "RocketLauncher":
                    prefabToDrop = RocketLauncherPickUpPrefab;
                    break;
                case "Bfg":
                    prefabToDrop = bfgPickUpPrefab;
                    break;
                case "Chainsaw":
                    prefabToDrop = ChainSawPickUpPrefab;
                    break;
                default:
                    Debug.LogWarning($"No prefab found for {slot.weaponName}.");
                    break;
            }

            if (prefabToDrop != null)
            {
                Instantiate(prefabToDrop, dropPosition, Quaternion.identity);
            }

            slot.weaponName = null;
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

    public void AddAmmo(int ammoAmount)
    {
        Transform hudTransform = GameObject.Find("HUD")?.transform;
        if (hudTransform == null) return;

        foreach (var slot in inventory)
        {
            if (!string.IsNullOrEmpty(slot.weaponName))
            {
                Transform weaponTransform = hudTransform.Find(slot.weaponName);
                if (weaponTransform != null)
                {
                    Gun gun = weaponTransform.GetComponent<Gun>();
                    if (gun != null)
                    {
                        int ammoToAdd = Mathf.Min(ammoAmount, gun.maxAmmo - gun.ammo);
                        gun.ammo += ammoToAdd;
                        // Debug.Log($"{gun.name} recibió {ammoToAdd} de munición. Total: {gun.ammo}/{gun.maxAmmo}");
                    }
                }
            }
        }
    }


}
