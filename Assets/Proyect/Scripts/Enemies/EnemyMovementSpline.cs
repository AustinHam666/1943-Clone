using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

public class EnemyMovementSpline : MonoBehaviour
{
    [Header("Configuración de Movimiento curvo")]
    [SerializeField] private float speed = 5f;

    // Variables de control interno
    private SplineContainer splinePath;
    private float progress = 0f;
    private float splineLength;

    // El Spawner llama a esta función cuando el enemigo nace
    public void SetSplinePath(SplineContainer path)
    {
        splinePath = path;
        splineLength = splinePath.CalculateLength();
        progress = 0f;

        if (splineLength <= 0)
        {
            Debug.LogError("¡ERROR! La curva mide 0. Asegúrate de haber dibujado puntos en el Path_Bucle.");
        }
    }

    private void Update()
    {
        // Si no hay ruta válida, el avión se queda quieto
        if (splinePath == null || splineLength <= 0) return;

        // Calculamos cuánto avanzar basándonos en la velocidad
        float distanceThisFrame = speed * Time.deltaTime;
        progress += distanceThisFrame / splineLength;

        // Si el progreso es menor o igual a 1, el avión sigue en la curva
        if (progress <= 1f)
        {
            // OBTENEMOS LA POSICIÓN Y LA CONVERTIMOS AL MUNDO REAL
            float3 localPos = splinePath.EvaluatePosition(progress);
            transform.position = splinePath.transform.TransformPoint((Vector3)localPos);

            // ROTACIÓN: Hacemos que la nariz del avión apunte hacia donde va la curva
            float3 tangent = splinePath.EvaluateTangent(progress);
            Vector3 worldTangent = splinePath.transform.TransformDirection((Vector3)tangent);

            if (worldTangent != Vector3.zero)
            {
                transform.up = worldTangent;
            }
        }
        else
        {
            // El avión llegó al final de la línea. Se desactiva y vuelve al Pooler.
            gameObject.SetActive(false);
        }
    }
}