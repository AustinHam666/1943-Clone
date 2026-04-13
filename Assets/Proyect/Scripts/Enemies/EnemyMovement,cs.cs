using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Configuración de Ruta")]
    [SerializeField] private float speed = 4f;
    [Tooltip("Distancia mínima para considerar que llegó a un punto")]
    [SerializeField] private float waypointTolerance = 0.1f;

    private Transform[] currentPath;
    private int currentWaypointIndex = 0;
    private bool hasPath = false;

    private void FixedUpdate()
    {
        if (!hasPath || currentPath == null || currentPath.Length == 0)
        {
            // Movimiento por defecto si no tiene ruta: ir hacia abajo
            transform.Translate(Vector3.down * speed * Time.fixedDeltaTime);
            return;
        }

        FollowPath();
    }

    /// <summary>
    /// El Spawner llamará a este método para asignarle una ruta cuando el enemigo nazca.
    /// </summary>
    public void SetPath(Transform[] newPath)
    {
        currentPath = newPath;
        currentWaypointIndex = 0;
        hasPath = true;

        // Colocamos al enemigo en el primer punto de la ruta inmediatamente
        if (currentPath.Length > 0)
        {
            transform.position = currentPath[0].position;
        }
    }

    private void FollowPath()
    {
        // 1. Obtenemos la posición del punto objetivo
        Transform targetWaypoint = currentPath[currentWaypointIndex];

        // 2. Movemos al enemigo hacia ese punto
        transform.position = Vector2.MoveTowards(transform.position, targetWaypoint.position, speed * Time.fixedDeltaTime);

        // Opcional: Hacer que el avión rote mirando hacia donde va (descomenta si tu sprite apunta hacia arriba)
        /*
        Vector2 direction = (targetWaypoint.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        */

        // 3. Comprobamos si ya llegamos al punto
        if (Vector2.Distance(transform.position, targetWaypoint.position) <= waypointTolerance)
        {
            currentWaypointIndex++; // Pasamos al siguiente punto

            // 4. Si llegamos al final de la ruta, desactivamos el enemigo (vuelve al Pool)
            if (currentWaypointIndex >= currentPath.Length)
            {
                hasPath = false;
                gameObject.SetActive(false);
            }
        }
    }
}