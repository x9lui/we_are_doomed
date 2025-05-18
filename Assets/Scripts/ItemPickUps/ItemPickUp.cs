using UnityEngine;

public abstract class ItemPickUp : MonoBehaviour
{
    protected InventoryScript playerInventory;
    protected GameObject player;
    public void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("No GameObject with tag 'Player' found in the scene!");
            return;
        }

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
            OnPickUp(other);
        }
    }
}
