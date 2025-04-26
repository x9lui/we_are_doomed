using UnityEngine;

// Example: Automatic Pistol
public class AutomaticPistol : Gun
{
    public override void Fire()
    {
        Debug.Log($"{gunName} fired!");
    }

    public override void Initialize()
    {
        base.Initialize();
        weaponType = InventoryManager.WeaponType.Pistol;
        gunName = "Automatic Pistol";
        ammoCapacity = 15;
        fireRate = 0.2f;
        Debug.Log("Automatic Pistol initialized with default values.");    
    }
}
