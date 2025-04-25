using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    // Define the number of slots and their types
    public enum WeaponType { None, Pistol, Rifle, Shotgun, RocketLauncher }
    public WeaponType[] weaponSlots = new WeaponType[5]; // 5 slots for weapons

    // Currently selected slot
    private int currentSlot = 0;

    // Start is called before the first execution of Update
    void Start()
    {
        // // Initialize all slots to None
        // for (int i = 0; i < weaponSlots.Length; i++)
        // {
        //     weaponSlots[i] = WeaponType.None;
        // }

        // // Example: Assign a default weapon to the first slot
        // weaponSlots[0] = WeaponType.Pistol;
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
        if (slotIndex >= 0 && slotIndex < weaponSlots.Length)
        {
            currentSlot = slotIndex;
            Debug.Log($"Switched to slot {slotIndex + 1}: {weaponSlots[slotIndex]}");
        }
    }

    // Add a weapon to a specific slot
    public bool AddWeaponToSlot(WeaponType weapon, int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < weaponSlots.Length && weaponSlots[slotIndex] == WeaponType.None)
        {
            weaponSlots[slotIndex] = weapon;
            Debug.Log($"Added {weapon} to slot {slotIndex + 1}");
            return true;
        }
        Debug.Log($"Slot {slotIndex + 1} is already occupied or invalid.");
        return false;
    }

    // Get the currently selected weapon
    public WeaponType GetCurrentWeapon()
    {
        return weaponSlots[currentSlot];
    }
}
