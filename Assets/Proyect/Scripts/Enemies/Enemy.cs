using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Enemy : MonoBehaviour
{
    [Header("Estadísticas Base")]
    [SerializeField] private float maxHealth = 10f;
    [SerializeField] private float damageToPlayer = 20f;
    [SerializeField] private int scoreValue = 100;

    [Header("Efectos Visuales")]
    [SerializeField] private GameObject explosionPrefab;

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

        if (currentHealth <= 0)
        {
            diedByPlayer = true;
            Die();
        }
    }

    private void Die()
    {
        // --- 1. AVISO AL PADRE (BARCO O BOSS) ---
        // Esto es lo que hace que el BattleshipMaster cuente la baja
        SendMessageUpwards("TorretaDestruida", SendMessageOptions.DontRequireReceiver);

        // --- 2. LÓGICA DE EXPLOSIÓN ---
        if (diedByPlayer && explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        // Sumamos puntos
        if (diedByPlayer && GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(scoreValue);
        }

        // --- 3. AVISO AL ESCUADRÓN ROJO ---
        if (mySquadron != null)
        {
            if (diedByPlayer)
            {
                mySquadron.ReportPlaneKilled(transform.position);
            }
        }

        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si el jugador nos choca
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(damageToPlayer);
                diedByPlayer = true;
                Die();
            }
        }
    }

    private void OnBecameInvisible()
    {
        if (!diedByPlayer)
        {
            if (mySquadron != null) mySquadron.ReportPlaneEscaped();
            Die();
        }
    }
}