using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

[RequireComponent(typeof(SpriteRenderer))]
public class EnemyMovementSpline : MonoBehaviour
{
    [Header("Configuración de Movimiento curvo")]
    [SerializeField] private float speed = 5f;

    [Header("Sprites Direccionales (BVD KAI)")]
    [Tooltip("Arrastrá los 16 sprites en sentido antihorario empezando por el que mira a la DERECHA")]
    public Sprite[] directionalSprites;

    // Variables de control interno
    private SplineContainer splinePath;
    private float progress = 0f;
    private float splineLength;
    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    // El Spawner llama a esta función cuando el enemigo nace
    public void SetSplinePath(SplineContainer path)
    {
        splinePath = path;
        if (splinePath != null)
        {
            splineLength = splinePath.CalculateLength();
        }
        progress = 0f;

        if (splineLength <= 0)
        {
            Debug.LogError("¡ERROR! La curva mide 0. Asegúrate de haber dibujado puntos en el Path_Bucle.");
        }
    }

    private void Update()
    {
        if (splinePath == null || splineLength <= 0) return;

        float distanceThisFrame = speed * Time.deltaTime;
        progress += distanceThisFrame / splineLength;

        if (progress <= 1f)
        {
            // 1. POSICIONAMIENTO
            float3 localPos = splinePath.EvaluatePosition(progress);
            transform.position = splinePath.transform.TransformPoint((Vector3)localPos);

            // 2. CÁLCULO DE DIRECCIÓN (Tangente)
            float3 tangent = splinePath.EvaluateTangent(progress);
            Vector3 worldTangent = splinePath.transform.TransformDirection((Vector3)tangent);

            if (worldTangent != Vector3.zero)
            {
                UpdateSpriteDirection(worldTangent);

                // OPCIONAL: Dejamos la rotación en 0 para que el objeto no gire, 
                // solo cambie el dibujo.
                transform.rotation = Quaternion.identity;
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

        // Calculamos el ángulo basado en la tangente de la curva
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        if (angle < 0) angle += 360f;

        float slice = 360f / directionalSprites.Length;
        int index = Mathf.FloorToInt((angle + (slice / 2f)) / slice);

        if (index >= directionalSprites.Length) index = 0;

        sr.sprite = directionalSprites[index];
    }
}