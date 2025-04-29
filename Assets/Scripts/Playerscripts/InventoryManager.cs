using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    // Define the number of slots and their types
    public enum WeaponType { None, Pistol, Rifle, Shotgun, Fist }

    public SpriteRenderer gunHolder;
    private Sprite GunSprite;
    // Class to represent a slot in the inventory
    [System.Serializable]
    public class InventorySlot
    {
        public WeaponType type; // The type of weapon (e.g., Pistol, Rifle)
        public Gun gun;         // The specific weapon instance (e.g., AutomaticPistol)
    }

    // List of inventory slots
    public List<InventorySlot> weaponSlots = new List<InventorySlot>();

    // Currently selected slot
    private int currentSlot = 0;

    // Start is called before the first execution of Update
    void Start()
    {
        // Initialize the inventory with empty slots
        // for (int i = 0; i < 5; i++)
        // {
        //     weaponSlots.Add(new InventorySlot { type = WeaponType.None, gun = null });
        // }

        // Assign a default weapon (Automatic Pistol) to the first slot
        // GameObject pistolPrefab = Resources.Load<GameObject>("Prefabs/Weapons/AutomaticPistol.prefab"); // Ensure the prefab is in a Resources folder
        // if (pistolPrefab != null)
        // {
        //     GameObject pistolInstance = Instantiate(pistolPrefab, transform);
        //     pistolInstance.SetActive(true);
        //     Debug.Log("Automatic Pistol instantiated successfully!");
        //     pistolInstance.transform.localPosition = Vector3.zero; // Adjust position as needed
        //     pistolInstance.transform.localRotation = Quaternion.identity;

        //     // Assign the weapon to the first slot
        //     Gun pistolGun = pistolInstance.GetComponent<Gun>();
        //     if (pistolGun != null)
        //     {
        //         weaponSlots[0].type = WeaponType.Pistol;
        //         weaponSlots[0].gun = pistolGun;
        //     }
        // }
        // else
        // {
        //     Debug.LogWarning("Automatic Pistol prefab not found in Resources/Weapons!");
        // }
            // Call Initialize on all weapons in the inventory
        foreach (var slot in weaponSlots)
        {
            if (slot.gun != null)
            {
                slot.gun.Initialize();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandleSlotSwitching();
    }

    // Handle switching between weapon slots
    private void HandleSlotSwitching()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SelectSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SelectSlot(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SelectSlot(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SelectSlot(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) SelectSlot(4);
    }

    // Select a weapon slot
    private void SelectSlot(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < weaponSlots.Count)
        {
            // Actualiza el slot actual
            currentSlot = slotIndex;
            InventorySlot selectedSlot = weaponSlots[currentSlot];

            // Elimina todas las instancias previas de armas
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            // Instancia y posiciona el arma seleccionada
            if (selectedSlot.gun != null)
            {
                GameObject gunInstance = Instantiate(selectedSlot.gun.gameObject, transform);
                gunInstance.name = selectedSlot.gun.gunName; // Opcional: Renombra la instancia para mayor claridad

                // Busca el SpriteRenderer en los hijos de la instancia
                SpriteRenderer spriteRenderer = gunInstance.GetComponentInChildren<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    GunSprite = spriteRenderer.sprite; // Obtiene el sprite del SpriteRenderer
                    gunHolder.sprite = GunSprite; // Cambia el sprite del gunHolder
                    gunHolder.transform.localPosition = selectedSlot.gun.possitionOffset; // Ajusta la posición del gunHolder
                    gunHolder.transform.localScale = selectedSlot.gun.scaleoffset; // Ajusta la escala del gunHolder
                }
                else
                {
                    Debug.LogWarning("No SpriteRenderer found in the gun instance or its children!");
                }

                gunInstance.SetActive(false); // Activa el arma seleccionada
            }

            // Mensaje de depuración
            Debug.Log($"Switched to slot {slotIndex + 1}: {selectedSlot.type} - {selectedSlot.gun?.gunName ?? "No weapon"}");
        }
    }

    // Add a weapon to a specific slot
    public bool AddWeaponToSlot(WeaponType weaponType, Gun weapon, int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < weaponSlots.Count && weaponSlots[slotIndex].type == WeaponType.None)
        {
            weaponSlots[slotIndex].type = weaponType;
            weaponSlots[slotIndex].gun = weapon;
            Debug.Log($"Added {weapon.gunName} to slot {slotIndex + 1}");
            return true;
        }
        Debug.Log($"Slot {slotIndex + 1} is already occupied or invalid.");
        return false;
    }

    // Get the currently selected weapon
    public Gun GetCurrentWeapon()
    {
        return weaponSlots[currentSlot].gun;
    }
}
