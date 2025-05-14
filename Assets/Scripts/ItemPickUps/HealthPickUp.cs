using UnityEngine;

public class HealthPickUp : ItemPickUp
{
    public float healthAmount = 20f; // Cantidad de armadura que restaura

    protected override void OnPickUp(Collider player)
    {
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.HealPlayer(healthAmount);
            Debug.Log($"Player picked up health. Current health: {playerHealth.CurrentHealth}");
        }else
        {
            Debug.LogError("PlayerHealth component not found on the player!");
        }
    }

}
