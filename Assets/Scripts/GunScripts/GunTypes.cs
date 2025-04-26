using UnityEngine;

public abstract class GunTypes : MonoBehaviour
{
    // General type of the weapon (e.g., Pistol, Rifle, etc.)
    public InventoryManager.WeaponType weaponType;

    // Common properties for all guns
    public string gunName;
    public int ammoCapacity;
    public float fireRate;

    // Abstract method for firing (to be implemented by specific guns)
    public abstract void Fire();

        // Custom initialization method
    public virtual void Initialize()
    {
        Debug.Log($"{gunName} initialized!");
    }
}