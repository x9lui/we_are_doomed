using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.InputSystem.Processors;

public class WeaponHUDDisplay : MonoBehaviour
{
    public List<TextMeshProUGUI> weaponTexts; // Textos numerados: 1,2,3,4,5, etc.
    public TextMeshProUGUI weaponNameText;    // Texto que muestra el nombre del arma actual

    public Color enabledColor;
    public Color disabledColor;
    public Color currentColor;


    public void UpdateHUD(List<InventoryScript.WeaponSlot> inventory)
    {
        for (int i = 0; i < weaponTexts.Count; i++)
        {
            // Evitar errores por desincronización entre inventory y weaponTexts
            if (i < inventory.Count)
            {
                var type = (InventoryScript.WeaponType)i;
                var slot = inventory.Find(s => s.weaponType == type);
                if (slot != null && !string.IsNullOrEmpty(slot.weaponName))
                {
                    weaponTexts[i].color = enabledColor;
                }
                else
                {
                    weaponTexts[i].color = disabledColor;
                }
            }
            else
            {
                // Si no hay datos en el inventario para esta posición, desactivar visualmente
                weaponTexts[i].color = disabledColor;
            }
        }
    }


    public void ActivateWeaponUI(InventoryScript.WeaponType type, string weaponName)
    {
        int index = (int)type;
        if (index >= 0 && index < weaponTexts.Count)
            weaponTexts[index].color = enabledColor;
    }

    public void DeactivateWeaponUI(InventoryScript.WeaponType type)
    {
        int index = (int)type;
        if (index >= 0 && index < weaponTexts.Count)
            weaponTexts[index].color = disabledColor;
    }

    public void HighlightCurrentWeapon(InventoryScript.WeaponType currentType, string currentWeaponName, List<InventoryScript.WeaponSlot> inventory)
    {
        for (int i = 0; i < weaponTexts.Count; i++)
        {
            var slot = inventory.Find(s => s.weaponType == (InventoryScript.WeaponType)i);

            if (slot != null && !string.IsNullOrEmpty(slot.weaponName))
            {
                // Si es el arma actual → color de arma activa
                weaponTexts[i].color = (i == (int)currentType) ? currentColor : enabledColor;
            }
            else
            {
                weaponTexts[i].color = disabledColor;
            }
        }

        switch (currentWeaponName)
        {
            case "Fist":
                weaponNameText.text = "Manos";
                break;
            case "Pistol2":
                weaponNameText.text = "Pistola";
                break;
            case "SemiPistol":
                weaponNameText.text = "Pistola Semiautomatica";
                break;
            case "EnergyGun":
                weaponNameText.text = "Escopeta de Energia";
                break;
            case "MachineGun":
                weaponNameText.text = "Subfusil";
                break;
            case "ShotgunNormal":
                weaponNameText.text = "Escopeta";
                break;
            case "PlasmaGun":
                weaponNameText.text = "Rifle de Plasma";
                break;
            case "Bfg":
                weaponNameText.text = "BFG 9000";
                break;
             case "RocketLauncher":
                weaponNameText.text = "LanzaCohetes";
                break;
            case "Chainsaw":
                weaponNameText.text = "Motosierra";
                break;
            default:
                weaponNameText.text = "Arma No Reconocida";
                break;
                
        }
    }
}
