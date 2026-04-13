using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    [Header("Configuración de Disparo")]
    [SerializeField] private string bulletPoolTag = "EnemyBullet";
    [SerializeField] private Transform firePoint;
    [SerializeField] private float timeBetweenShots = 1.5f;

    private float shotTimer;
    private Transform playerTransform;

    private void Start()
    {
        // Buscamos al jugador en la escena por su Tag
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }

        // Añadimos un poco de aleatoriedad al primer disparo para que 
        // si entran 5 aviones juntos, no disparen los 5 a la vez.
        shotTimer = Random.Range(0.5f, timeBetweenShots);
    }

    private void Update()
    {
        // Si el jugador está muerto o desactivado, no hacemos nada
        if (playerTransform == null || !playerTransform.gameObject.activeInHierarchy) return;

        shotTimer -= Time.deltaTime;

        if (shotTimer <= 0f)
        {
            ShootAtPlayer();
            shotTimer = timeBetweenShots;
        }
    }

    private void ShootAtPlayer()
    {
        if (ObjectPooler.Instance == null || firePoint == null) return;

        // 1. Calculamos la dirección hacia el jugador
        Vector2 direction = (playerTransform.position - firePoint.position).normalized;

        // 2. Calculamos el ángulo para que la bala mire hacia el jugador
        // Restamos 90 grados porque el "frente" de nuestros sprites en 2D suele ser el eje Y (Up)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        Quaternion bulletRotation = Quaternion.Euler(0, 0, angle);

        // 3. Disparamos la bala desde el Pool
        ObjectPooler.Instance.SpawnFromPool(bulletPoolTag, firePoint.position, bulletRotation);
    }
}