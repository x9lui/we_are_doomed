using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float MaxHealth = 100f; // Maximum health of the player
    public float CurrentHealth; // Current health of the player
    public float MaxArmor = 100f; // Maximum armor of the player
    public float CurrentArmor; // Current armor of the player
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public InterfaceHealthArmor healthBarUI; // Reference to the health bar UI script
    public InterfaceHealthArmor armorBarUI; // Reference to the armor bar UI script
    public GameObject deathPanelUI;         // Reference to the death panel UI

    public bool UnJugador;

    void Start()
    {
        CurrentHealth = MaxHealth; // Initialize current health to maximum health
        CurrentArmor = MaxArmor; // Initialize current armor to maximum armor
    }

    public void TakeDamage(float damage)
    {
        if (CurrentArmor > 0) // Check if the player has armor
        {
            CurrentArmor -= damage; // Reduce armor by damage amount

            armorBarUI.SetProgreso(CurrentArmor/MaxArmor); // Update armor bar UI

            if (CurrentArmor < 0) // If armor goes below zero, reduce health
            {
                CurrentHealth += CurrentArmor; // Add the negative armor value to health
                CurrentArmor = 0; // Set armor to zero
                healthBarUI.SetProgreso(CurrentHealth /MaxHealth);

            }
        }
        else // If no armor left, reduce health directly
        {
            healthBarUI.SetProgreso(CurrentHealth /MaxHealth);
            CurrentHealth -= damage; // Reduce health by damage amount
        }

        if (CurrentHealth <= 0) // Check if health is zero or below
        {
            Die(); // Call the Die method to handle player death
        }
    }
    public void HealPlayer(float amount)
    {
        CurrentHealth += amount; // Increase health by the healing amount
        if (CurrentHealth > MaxHealth) // Check if health exceeds maximum health
        {
            CurrentHealth = MaxHealth; // Set health to maximum health
        }
    }

    public void Die(){
        // Handle player death (e.g., play animation, disable controls, etc.)
        Debug.Log("Player has died!"); // Log player death
        
        if (deathPanelUI != null)
            deathPanelUI.SetActive(true); // Mostrar el menú de muerte

        if(UnJugador)
        {
            Time.timeScale = 0f;
        }
        
    }
}
