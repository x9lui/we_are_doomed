using UnityEngine;

public class BfgProjectile : MonoBehaviour
{
    public float speed = 20f;
    public float explosionDelay = 3f;
    public float explosionRadius = 20f;
    public float explosionForce = 5000f;
    public float damage = 200f;
    public GameObject explosionEffect;
    public GameObject plasmaProjectilePrefab;
    public Transform firePoint;
    private AudioSource audioSource;
    private float timer;
    private AudioClip ExplosionSound;
    [System.Obsolete]
    void Start()
    {
        timer = explosionDelay;
        GetComponent<Rigidbody>().velocity = transform.forward * speed;
        audioSource = GetComponent<AudioSource>();
        ExplosionSound = audioSource.clip;
        GameObject player = GameObject.FindWithTag("Player");
        Collider rocketCol = GetComponent<Collider>();
        if (player != null && rocketCol != null)
        {
            Collider playerCol = player.GetComponent<Collider>();
            if (playerCol != null)
                Physics.IgnoreCollision(rocketCol, playerCol, true);
        }

        BfgProjectile[] allProjectiles = FindObjectsOfType<BfgProjectile>();
        foreach (var other in allProjectiles)
        {
            if (other == this) continue;
            Collider otherCol = other.GetComponent<Collider>();
            if (otherCol != null && rocketCol != null)
                Physics.IgnoreCollision(rocketCol, otherCol, true);
        }

        if (plasmaProjectilePrefab != null && firePoint != null)
        {
            Debug.Log("Instanciando proyectil de plasma");
            float spawnOffset = 0.5f;
            Vector3 spawnPos = firePoint.position + firePoint.forward * spawnOffset;
            GameObject plasma = Instantiate(plasmaProjectilePrefab, spawnPos, firePoint.rotation);
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
        if (audioSource != null && ExplosionSound != null)
        {
            GameObject tempAudio = new GameObject("TempRocketExplosionAudio");
            tempAudio.transform.position = transform.position;
            AudioSource tempSource = tempAudio.AddComponent<AudioSource>();
            tempSource.clip = ExplosionSound;
            tempSource.Play();
            Destroy(tempAudio, ExplosionSound.length);
        }

        // Daño en área
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider nearby in colliders)
        {
            Enemy enemy = nearby.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                continue; // No aplicar fuerza a enemigos
            }
            PlayerHealth player = nearby.GetComponent<PlayerHealth>();
            if (player != null)
            {
                player.TakeDamage(damage);
                
            }
            Rigidbody rb = nearby.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }

        Destroy(gameObject);
    }
}