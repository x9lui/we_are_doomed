using UnityEngine;

public class EnemyAwareness : MonoBehaviour
{

    public bool isAgro;
    public void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            Debug.Log("Enemy is aware of the player!");
            isAgro = true;
        }
    }

    public void Update()
    {
        if(isAgro)
        {

        }
        else
        {

        }
    }
}
