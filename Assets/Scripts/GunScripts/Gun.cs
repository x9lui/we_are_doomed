using System.Collections.Generic;
using UnityEngine;

public abstract class Gun : MonoBehaviour
{
    public const int INFINITE_AMMO = -1; // Representa munición infinita
    public InventoryManager.WeaponType weaponType;
    public string gunName;
    public int ammoCapacity;
    public float fireRate;
    // public Vector3 possitionOffset;
    // public Vector3 scaleoffset;

    // Abstract method for firing
    public abstract void Fire();

    // Custom initialization method
    public virtual void Initialize()
    {
        Debug.Log($"{gunName} initialized!");
    }

    // Método para verificar si el arma tiene munición infinita
    public bool HasInfiniteAmmo()
    {
        return ammoCapacity == INFINITE_AMMO;
    }
}