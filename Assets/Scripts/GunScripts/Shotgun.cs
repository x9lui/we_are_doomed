using UnityEngine;

// Example: Automatic Pistol
public class Shotgun : Gun
{
    public override void Fire()
    {
        Debug.Log($"{gunName} fired!");
    }

    public override void Initialize()
    {
        base.Initialize();
        weaponType = InventoryManager.WeaponType.Shotgun;
        gunName = "Shotgun";
        ammoCapacity = 15;
        fireRate = 0.2f;
        possitionOffset = new Vector3(0.04f, -0.82f, 0);
        scaleoffset = new Vector3(2, 2, 2);
        Debug.Log("Shotgun initialized with default values.");    
    }
}
