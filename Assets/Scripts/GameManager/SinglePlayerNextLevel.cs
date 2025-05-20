using UnityEngine;

public class SinglePlayerNextLevel : MonoBehaviour
{
    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            SinglePlayerGameManager.Instance.NextLevel();
            Destroy(gameObject);
        }
    }
}
