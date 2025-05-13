using UnityEngine;

public abstract class ItemPickUp : MonoBehaviour
{
    protected InventoryScript playerInventory;
    protected GameObject player;
    public void Start()
    {
        // Obtener el GameObject del jugador usando la etiqueta "Player"
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("No GameObject with tag 'Player' found in the scene!");
            return;
        }

        // Obtener el componente InventoryScript del jugador
        playerInventory = player.GetComponent<InventoryScript>();
        if (playerInventory == null)
        {
            Debug.LogError("No InventoryScript found on the Player GameObject!");
        }
    }

    protected abstract void OnPickUp(Collider player);

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnPickUp(other); // Llamar al comportamiento específico del pickup
            Destroy(gameObject); // Destruir el objeto después de recogerlo
        }
    }
}
