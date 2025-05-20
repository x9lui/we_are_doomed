using UnityEngine;

public class SpriteRotator : MonoBehaviour
{
    private Transform target;
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        transform.LookAt(target);
    }
}
