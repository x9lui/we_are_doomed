using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System;

public class Enemy : MonoBehaviour
{
    public GameObject gunHitEffect;
    public EnemyManager enemyManager;
    public float enemyHealth;
    private Animator spriteAnim;
    private AngleToPlayer angleToPlayer;
    public AudioSource audioSource;
    public AudioClip[] sounds;
    private enum SoundType { Hit = 0, Death = 1, Fight = 2}

    [Header("Attack Settings")]
    public float attackDamage = 10f; // Daño que inflige el enemigo
    public float attackCooldown = 1.5f; // Tiempo entre ataques
    private float lastAttackTime; // Tiempo del último ataque
    private bool hasAppliedAttackDamage = false; // Indica si el daño ya fue aplicado

    private PlayerHealth playerHealth; // Referencia al componente PlayerHealth

    public Sprite attackSprite; // Asigna 'Imp-from-Doom-Spritesheet_30' en el inspector

    void Start()
    {
        spriteAnim = GetComponentInChildren<Animator>();
        angleToPlayer = GetComponent<AngleToPlayer>();
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
    }

    void Update()
    {
        Debug.Log("Angle: " + angleToPlayer.lastIndex);
        spriteAnim.SetFloat("SpriteRotation", angleToPlayer.lastIndex);
    }

    public void TakeDamage(float damage)
    {
        enemyHealth -= damage;
        if (enemyHealth > 0)
        {
            PlaySound(SoundType.Hit);
        }
        Instantiate(gunHitEffect, transform.position, Quaternion.identity);
        Debug.Log($"Enemy took damage: {damage}. Remaining health: {enemyHealth}");
        if (enemyHealth <= 0)
        {
            PlaySound(SoundType.Death);
            Destroy(gameObject);
        }
    }

    public void AttackPlayer()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            if (spriteAnim != null)
                spriteAnim.SetTrigger("Attack");

            hasAppliedAttackDamage = false;
            StartCoroutine(WaitForAttackSprite());
        }
    }

    private IEnumerator WaitForAttackSprite()
    {
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        while (!hasAppliedAttackDamage)
        {
            if (sr != null && sr.sprite == attackSprite)
            {
                if (playerHealth != null)
                {
                    PlaySound(SoundType.Fight);
                    playerHealth.TakeDamage(attackDamage);
                    Debug.Log($"Player took {attackDamage} damage from enemy.");
                }
                hasAppliedAttackDamage = true;
            }
            yield return null;
        }
    }

    public void Knockback(Vector3 force)
    {
        var agent = GetComponent<NavMeshAgent>();
        var rb = GetComponent<Rigidbody>();

        if (agent != null && rb != null)
        {
            agent.enabled = false;
            rb.isKinematic = false;

            float knockbackMultiplier = 0.1f; // Ajusta este valor para más o menos fuerza
            rb.AddForce(force * knockbackMultiplier, ForceMode.Impulse);

            StartCoroutine(ReenableNavMeshAgent(0.5f));
        }
    }

    private IEnumerator ReenableNavMeshAgent(float delay)
    {
        yield return new WaitForSeconds(delay);
        var agent = GetComponent<NavMeshAgent>();
        var rb = GetComponent<Rigidbody>();
        if (agent != null && rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
            agent.enabled = true;
            agent.Warp(transform.position);
        }
    }

    private void PlaySound(SoundType type)
    {
        int index = (int)type;
        if (index >= 0 && index < sounds.Length)
        {
            audioSource.PlayOneShot(sounds[index], 0.5f);
        }
        else
        {
            Debug.LogWarning("Índice de sonido fuera de rango.");
        }
    }
}
