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

    [Header("Mecánica de Escuadrón")]
    public RedSquadronManager mySquadron;

    private float currentHealth;
    private bool diedByPlayer;
    private bool yaMurio; // Evita que Die() se llame dos veces

    private void OnEnable()
    {
        currentHealth = maxHealth;
        diedByPlayer = false;
        yaMurio = false;
    }

    public void TakeDamage(float damageAmount)
    {
        if (yaMurio) return;
        currentHealth -= damageAmount;
        if (currentHealth <= 0)
        {
            diedByPlayer = true;
            Die();
        }
    }

    private void Die()
    {
        if (yaMurio) return;
        yaMurio = true;

        LevelManager.ReportarMuerte();
        SendMessageUpwards("TorretaDestruida", SendMessageOptions.DontRequireReceiver);

        if (diedByPlayer)
        {
            // Explosión siempre al morir por el jugador
            if (explosionPrefab != null)
                Instantiate(explosionPrefab, transform.position, Quaternion.identity);

            if (GameManager.Instance != null)
                GameManager.Instance.AddScore(scoreValue);

            if (mySquadron != null)
                mySquadron.ReportPlaneKilled(transform.position);
        }
        else
        {
            if (mySquadron != null)
                mySquadron.ReportPlaneEscaped();
        }

        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (yaMurio) return;
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(damageToPlayer);
                diedByPlayer = false;
                Die();
            }
        }
    }

    private void OnBecameInvisible()
    {
        if (yaMurio) return;
        diedByPlayer = false;
        Die();
    }
}