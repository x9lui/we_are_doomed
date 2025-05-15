using UnityEngine;

public class ArmorPickUp : ItemPickUp
{
    public float armorAmount = 20f; // Cantidad de armadura que restaura

    protected override void OnPickUp(Collider player)
    {
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.ArmorPlayer(armorAmount);
            Debug.Log($"Player picked up armor. Current armor: {playerHealth.CurrentArmor}");
            Destroy(gameObject); // Destruir el objeto después de recogerlo

        }else
        {
            Debug.LogError("PlayerHealth component not found on the player!");
        }
    }
}
