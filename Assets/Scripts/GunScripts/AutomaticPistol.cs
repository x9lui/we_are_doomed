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
        // possitionOffset = new Vector3(-0.09f, -0.75f, 0);
        // scaleoffset = new Vector3(1, 1, 1);
        // Set the sprite for the gun holder
        Debug.Log("Automatic Pistol initialized with default values.");    
    }
}
