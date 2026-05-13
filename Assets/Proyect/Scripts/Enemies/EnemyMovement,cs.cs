using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Configuración de Ruta")]
    [SerializeField] private float speed = 4f;
    [SerializeField] private float waypointTolerance = 0.1f;

    private Transform[] currentPath;
    private int currentWaypointIndex = 0;
    private bool hasPath = false;

    private void OnEnable()
    {
        // Reseteo completo al salir del pool
        currentWaypointIndex = 0;
        hasPath = false;
        currentPath = null;
    }

    private void FixedUpdate()
    {
        if (!hasPath || currentPath == null || currentPath.Length == 0)
        {
            transform.Translate(Vector3.down * speed * Time.fixedDeltaTime);
            if (transform.position.y < -8f)
                gameObject.SetActive(false);
            return;
        }
        FollowPath();
    }

    public void SetPath(Transform[] newPath)
    {
        currentPath = newPath;
        currentWaypointIndex = 0;
        hasPath = true;

        if (currentPath != null && currentPath.Length > 0)
            transform.position = currentPath[0].position;
    }

    private void FollowPath()
    {
        if (currentWaypointIndex >= currentPath.Length)
        {
            hasPath = false;
            gameObject.SetActive(false);
            return;
        }

        Transform targetWaypoint = currentPath[currentWaypointIndex];
        transform.position = Vector2.MoveTowards(
            transform.position,
            targetWaypoint.position,
            speed * Time.fixedDeltaTime
        );

        if (Vector2.Distance(transform.position, targetWaypoint.position) <= waypointTolerance)
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= currentPath.Length)
            {
                hasPath = false;
                gameObject.SetActive(false);
            }
        }
    }
}