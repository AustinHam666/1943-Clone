using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Enemy : MonoBehaviour
{
    [Header("Estadísticas Base")]
    [SerializeField] private float maxHealth = 10f;
    [SerializeField] private float damageToPlayer = 20f;
    [SerializeField] private int scoreValue = 100;

    [Header("Efectos Visuales")]
    [SerializeField] private GameObject explosionPrefab; // <-- Arrastrá acá tu prefab de explosión

    [Header("Mecánica de Escuadrón (Solo para Zako Rojos)")]
    public RedSquadronManager mySquadron;

    private float currentHealth;
    private bool diedByPlayer;

    private void OnEnable()
    {
        currentHealth = maxHealth;
        diedByPlayer = false;
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        // TODO: Añadir efecto de parpadeo blanco (después lo vemos si querés)

        if (currentHealth <= 0)
        {
            diedByPlayer = true;
            Die();
        }
    }

    private void Die()
    {
        // --- LÓGICA DE EXPLOSIÓN ---
        // Solo explotamos si el jugador lo mató (bala o choque)
        if (diedByPlayer && explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        // Si el jugador lo mató, sumamos puntos
        if (diedByPlayer && GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(scoreValue);
        }

        // --- AVISO AL ESCUADRÓN ROJO ---
        if (mySquadron != null)
        {
            if (diedByPlayer)
            {
                mySquadron.ReportPlaneKilled(transform.position);
            }
        }

        gameObject.SetActive(false); // Vuelve al Pool
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(damageToPlayer);
                diedByPlayer = true; // El choque kamikaze explota
                Die();
            }
        }
    }

    private void OnBecameInvisible()
    {
        if (!diedByPlayer)
        {
            // Agregamos esta línea para avisar que se escapó
            if (mySquadron != null) mySquadron.ReportPlaneEscaped();

            Die();
        }
    }
}