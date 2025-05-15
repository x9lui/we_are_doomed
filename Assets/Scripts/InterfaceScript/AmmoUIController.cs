using UnityEngine;
using UnityEngine.UI;

public class AmmoUIController : MonoBehaviour
{
    [Header("Referencia al contenedor de armas")]
    public GameObject weaponHolder; // El GameObject que contiene todas las armas como hijos

    [Header("Sprites de los dígitos (0-9)")]
    public Sprite[] digitSprites;

    [Header("UI - Munición Actual")]
    public Image currentAmmoDigit1;
    public Image currentAmmoDigit2;
    public Image currentAmmoDigit3;

    [Header("UI - Munición Máxima")]
    public Image maxAmmoDigit1;
    public Image maxAmmoDigit2;
    public Image maxAmmoDigit3;

    private Gun activeGun;

    void Update()
    {
        UpdateActiveGun();
        if (activeGun == null) return;

        UpdateAmmoDisplay(activeGun.GetAmmo(), currentAmmoDigit1, currentAmmoDigit2, currentAmmoDigit3);
        UpdateAmmoDisplay(activeGun.GetMaxAmmo(), maxAmmoDigit1, maxAmmoDigit2, maxAmmoDigit3);
    }

    void UpdateActiveGun()
    {
        // Buscar entre los hijos del holder cuál arma está activa
        foreach (Transform weapon in weaponHolder.transform)
        {
            if (weapon.gameObject.activeSelf)
            {
                Gun gun = weapon.GetComponent<Gun>();
                if (gun != null)
                {
                    if (activeGun != gun) // Solo si cambió de arma
                    {
                        activeGun = gun;
                    }
                    return;
                }
            }
        }

        // Si ninguna arma está activa
        activeGun = null;
    }

    void UpdateAmmoDisplay(int number, Image digit1, Image digit2, Image digit3)
    {
        number = Mathf.Clamp(number, 0, 999);

        int d1 = (number / 100) % 10;
        int d2 = (number / 10) % 10;
        int d3 = number % 10;

        digit1.sprite = digitSprites[d1];
        digit2.sprite = digitSprites[d2];
        digit3.sprite = digitSprites[d3];
    }
}
