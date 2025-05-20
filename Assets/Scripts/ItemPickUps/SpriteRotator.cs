using UnityEngine;

public class SpriteRotator : MonoBehaviour
{
    private Transform target;

    void Start()
    {
        TryFindPlayer();
    }

    void Update()
    {
        if (target == null)
        {
            TryFindPlayer();
            if (target == null) return;
        }
        transform.LookAt(target);
    }

    private void TryFindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            target = playerObj.transform;
    }
}
