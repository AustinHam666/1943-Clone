using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    [Header("Configuración de Disparo")]
    [SerializeField] private string bulletPoolTag = "EnemyBullet";
    [SerializeField] private Transform firePoint;
    [SerializeField] private float timeBetweenShots = 1.5f;

    private float shotTimer;
    private Transform playerTransform;
    private Camera mainCam;

    private void Start()
    {
        mainCam = Camera.main;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }

        shotTimer = Random.Range(0.5f, timeBetweenShots);
    }

    private void Update()
    {
        if (playerTransform == null || !playerTransform.gameObject.activeInHierarchy) return;

        // NUEVO: Solo procesamos el disparo si el avión está dentro de la cámara
        if (IsOnScreen())
        {
            shotTimer -= Time.deltaTime;

            if (shotTimer <= 0f)
            {
                ShootAtPlayer();
                shotTimer = timeBetweenShots;
            }
        }
    }

    // Función para detectar si el enemigo entró al área de visión
    private bool IsOnScreen()
    {
        Vector3 screenPoint = mainCam.WorldToViewportPoint(transform.position);
        // Retorna verdadero si está entre 0 y 1 en X e Y (dentro del cuadro de la cámara)
        return screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
    }

    private void ShootAtPlayer()
    {
        if (ObjectPooler.Instance == null || firePoint == null) return;

        Vector2 direction = (playerTransform.position - firePoint.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        Quaternion bulletRotation = Quaternion.Euler(0, 0, angle);

        ObjectPooler.Instance.SpawnFromPool(bulletPoolTag, firePoint.position, bulletRotation);
    }
}