using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Enemy : MonoBehaviour
{
    [Header("Estadísticas Base")]
    [SerializeField] private float maxHealth = 10f;
    [SerializeField] private float damageToPlayer = 20f;

    // Preparando el terreno para la UI más adelante
    [SerializeField] private int scoreValue = 100;

    private float currentHealth;

    private void OnEnable()
    {
        // Como también usaremos Object Pooling para los enemigos, 
        // reiniciamos la salud cada vez que se activan.
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;

        // TODO: Añadir efecto de parpadeo blanco al recibir daño

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Le enviamos el valor de los puntos al GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(scoreValue);
        }

        gameObject.SetActive(false);
    }

    // Detectamos la colisión física contra el jugador
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si chocamos contra el jugador...
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                // Le hacemos daño al jugador y destruimos este avión (kamikaze)
                player.TakeDamage(damageToPlayer);
                Die();
            }
        }
    }
}