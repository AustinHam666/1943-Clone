using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

[RequireComponent(typeof(SpriteRenderer))]
public class EnemyMovementSpline : MonoBehaviour
{
    [Header("Configuración de Movimiento curvo")]
    [SerializeField] private float speed = 5f;

    [Header("Sprites Direccionales")]
    public Sprite[] directionalSprites;

    [Header("Mirada al jugador")]
    [Tooltip("Si está activo, el sprite apunta hacia el jugador en vez de seguir la tangente")]
    [SerializeField] private bool mirarAlJugador = false;

    private SplineContainer splinePath;
    private float progress = 0f;
    private float splineLength;
    private SpriteRenderer sr;
    private Transform playerTransform;
    private Camera mainCam;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        progress = 0f;
        splineLength = 0f;
        splinePath = null;

        // Buscamos al jugador
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;
        mainCam = Camera.main;
    }

    public void SetSplinePath(SplineContainer path)
    {
        splinePath = path;
        progress = 0f;

        if (splinePath != null)
        {
            splineLength = splinePath.CalculateLength();
            if (splineLength <= 0)
                Debug.LogError("[EnemyMovementSpline] La curva mide 0.");
        }
    }

    private bool IsOnScreen()
    {
        if (mainCam == null) return false;
        Vector3 screenPoint = mainCam.WorldToViewportPoint(transform.position);
        return screenPoint.z > 0 &&
               screenPoint.x > -0.1f && screenPoint.x < 1.1f &&
               screenPoint.y > -0.1f && screenPoint.y < 1.1f;
    }

    private void Update()
    {
        if (splinePath == null || splineLength <= 0) return;

        progress += (speed * Time.deltaTime) / splineLength;

        if (progress <= 1f)
        {
            // Posición
            float3 localPos = splinePath.EvaluatePosition(progress);
            transform.position = splinePath.transform.TransformPoint((Vector3)localPos);
            transform.rotation = Quaternion.identity;

            // Dirección del sprite
            if (mirarAlJugador && playerTransform != null)
            {
                if (playerTransform == null)
                {
                    GameObject player = GameObject.FindGameObjectWithTag("Player");
                    if (player != null) playerTransform = player.transform;
                }
                Vector3 dirAlJugador = playerTransform.position - transform.position;
                UpdateSpriteDirection(dirAlJugador);
            }
            else
            {
                float3 tangent = splinePath.EvaluateTangent(progress);
                Vector3 worldTangent = splinePath.transform.TransformDirection((Vector3)tangent);
                if (worldTangent != Vector3.zero)
                    UpdateSpriteDirection(worldTangent);
            }
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void UpdateSpriteDirection(Vector3 dir)
    {
        if (directionalSprites == null || directionalSprites.Length == 0) return;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360f;

        float slice = 360f / directionalSprites.Length;
        int index = Mathf.FloorToInt((angle + (slice / 2f)) / slice);
        if (index >= directionalSprites.Length) index = 0;

        sr.sprite = directionalSprites[index];
    }
}