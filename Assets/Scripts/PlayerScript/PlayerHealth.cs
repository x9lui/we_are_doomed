using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float MaxHealth = 100f; // Maximum health of the player
    public float CurrentHealth; // Current health of the player
    public float MaxArmor = 120f; // Maximum armor of the player
    public float CurrentArmor; // Current armor of the player

    public InterfaceHealthArmor healthBarUI; 
    public InterfaceHealthArmor armorBarUI;
    public GameObject deathPanelUI;         

    [SerializeField] private MouseLook _mouseLookScript;

    public InterfaceHead interfaceHead; 

    public bool UnJugador;

    void Start()
    {
        CurrentHealth = MaxHealth;
        CurrentArmor = MaxArmor;

        healthBarUI.SetProgreso(CurrentHealth / MaxHealth);
        interfaceHead.SetArmadura(CurrentArmor / MaxArmor);
    }

    public void TakeDamage(float damage)
    {
        if (CurrentArmor > 0)
        {
            CurrentArmor -= damage;

            armorBarUI.SetProgreso(CurrentArmor/MaxArmor);

            if (CurrentArmor < 0)
            {
                CurrentHealth += CurrentArmor;
                CurrentArmor = 0;
                healthBarUI.SetProgreso(CurrentHealth /MaxHealth);

            }
        }
        else
        {
            healthBarUI.SetProgreso(CurrentHealth /MaxHealth);
            CurrentHealth -= damage;
        }
        if (interfaceHead != null){
            interfaceHead.SetSalud(CurrentHealth / MaxHealth);
        }
        if (interfaceHead != null){
            interfaceHead.SetArmadura(CurrentArmor / MaxArmor);
        }
        if (CurrentHealth <= 0)
        {
            Die();
        }
    }
    public void HealPlayer(float amount)
    {
        CurrentHealth += amount;
        if (CurrentHealth > MaxHealth)
        {
            CurrentHealth = MaxHealth;
        }
        healthBarUI.SetProgreso(CurrentHealth / MaxHealth);

        if (interfaceHead != null)
            interfaceHead.SetSalud(CurrentHealth / MaxHealth);
            
    }

    public void ArmorPlayer(float amount)
    {
        CurrentArmor += amount;
        if (CurrentArmor > MaxArmor)
        {
            CurrentArmor = MaxArmor;
        }
        armorBarUI.SetProgreso(CurrentArmor / MaxArmor);
        
        if (interfaceHead != null)
            interfaceHead.SetArmadura(CurrentArmor/MaxArmor);
    }



    public void Die()
    {
        Debug.Log("Player has died!"); // Log player death

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _mouseLookScript.enabled = false;

        if (deathPanelUI != null)
            deathPanelUI.SetActive(true); // Mostrar el menú de muerte

        if (UnJugador)
        {
            Time.timeScale = 0f;
        }        
    }
}
