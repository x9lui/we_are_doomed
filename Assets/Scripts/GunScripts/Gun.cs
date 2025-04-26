using UnityEngine;

public abstract class Gun : MonoBehaviour
{
    public InventoryManager.WeaponType weaponType;
    public string gunName;
    public int ammoCapacity;
    public float fireRate;

    // Abstract method for firing
    public abstract void Fire();

    // Custom initialization method
    public virtual void Initialize()
    {
        Debug.Log($"{gunName} initialized!");
    }
}