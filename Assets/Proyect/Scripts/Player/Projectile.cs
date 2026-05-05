using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifeTime = 2f;
    [SerializeField] private float damage = 10f;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    private void OnEnable()
    {
        Invoke(nameof(Deactivate), lifeTime);
    }

    private void FixedUpdate()
    {
        rb.velocity = transform.up * speed;
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // --- EL DETECTOR DE MENTIRAS ---
        Debug.Log("<color=cyan>La bala chocó con: </color>" + collision.gameObject.name + " | Tag: " + collision.tag);

        // 1. Detección de Enemigos
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Deactivate();
            }
        }
        // 2. Detección del Power-Up
        else if (collision.CompareTag("PowerUp"))
        {
            ItemPow pow = collision.GetComponent<ItemPow>();
            if (pow != null)
            {
                pow.TakeHit(); // Hacemos que cambie de tipo
                Deactivate();  // La bala se destruye al impactar el ítem
            }
        }
    }

    // --- EL ARREGLO PARA LAS BALAS FANTASMA ---
    private void OnBecameInvisible()
    {
        Deactivate();
    }
}