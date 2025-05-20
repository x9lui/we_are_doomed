using UnityEngine;

public class SinglePlayerNextLevel : MonoBehaviour
{

    /*We need this flag because the gameObject is not destroyed instantly
    and that can lead to multiple calls to NextLevel().*/
    private bool triggered = false;
    void OnTriggerEnter(Collider collider)
    {
        if (triggered) return;
        
        if (collider.gameObject.CompareTag("Player"))
        {
            triggered = true;
            SinglePlayerGameManager.Instance.NextLevel();
            Destroy(gameObject);
        }
    }
}
