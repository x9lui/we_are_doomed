using UnityEngine;

public class RocketProjectile : MonoBehaviour
{
    public float speed = 20f;
    public float explosionDelay = 3f;
    public float explosionRadius = 5f;
    public float explosionForce = 700f;
    public float damage = 50f;
    public GameObject explosionEffect;

    private float timer;

    [System.Obsolete]
    void Start()
    {
        timer = explosionDelay;
        GetComponent<Rigidbody>().velocity = transform.forward * speed;

        // Ignorar colisión con el jugador
        GameObject player = GameObject.FindWithTag("Player");
        Collider rocketCol = GetComponent<Collider>();
        if (player != null && rocketCol != null)
        {
            Collider playerCol = player.GetComponent<Collider>();
            if (playerCol != null)
                Physics.IgnoreCollision(rocketCol, playerCol, true);
        }

        // Ignorar colisión con otros proyectiles
        RocketProjectile[] allProjectiles = FindObjectsOfType<RocketProjectile>();
        foreach (var other in allProjectiles)
        {
            if (other == this) continue;
            Collider otherCol = other.GetComponent<Collider>();
            if (otherCol != null && rocketCol != null)
                Physics.IgnoreCollision(rocketCol, otherCol, true);
        }
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            Explode();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Explode();
    }

    void Explode()
    {
        // Efecto visual
        if (explosionEffect != null)
            Instantiate(explosionEffect, transform.position, Quaternion.identity);

        // Daño en área
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider nearby in colliders)
        {
            // Daño a enemigos
            Enemy enemy = nearby.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            // Daño al jugador (opcional)
            PlayerHealth player = nearby.GetComponent<PlayerHealth>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
            // Física
            Rigidbody rb = nearby.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }

        Destroy(gameObject);
    }
}