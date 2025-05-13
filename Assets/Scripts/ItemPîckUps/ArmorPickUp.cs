using UnityEngine;

public class ArmorPickUp : ItemPickUp
{
    public float armorAmount = 20f; // Cantidad de armadura que restaura

    protected override void OnPickUp(Collider player)
    {
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.CurrentArmor = Mathf.Min(playerHealth.CurrentArmor + armorAmount, playerHealth.MaxArmor);
            Debug.Log($"Player picked up armor. Current armor: {playerHealth.CurrentArmor}");
        }
    }
}
