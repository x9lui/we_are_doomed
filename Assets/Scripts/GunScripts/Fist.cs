using UnityEngine;

// Example: Automatic Pistol
public class Fist : Gun
{
    public override void Fire()
    {
        Debug.Log($"{gunName} fired!");
    }

    public override void Initialize()
    {
        base.Initialize();
        weaponType = InventoryManager.WeaponType.Shotgun;
        gunName = "Fist";
        ammoCapacity = INFINITE_AMMO; // Munición infinita
        fireRate = 0.2f;
        possitionOffset = new Vector3(-0.68f, -0.82f, -0.01f);
        scaleoffset = new Vector3(1, 1, 1);
        Debug.Log("Fist initialized with infinite ammo.");
    }
}
