using System.Collections;
using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

public class EnemySpawnerSpline : MonoBehaviour
{
    [Header("Configuración de la Oleada Curva")]
    [SerializeField] private string enemyPoolTag = "EnemyAcrobata";
    [SerializeField] private int enemiesInWave = 5;
    [SerializeField] private float timeBetweenSpawns = 0.3f;

    [Tooltip("Arrastra aquí tu objeto Path_Bucle")]
    [SerializeField] private SplineContainer splinePath;

    // Función que llama el LevelManager
    public void TriggerWave()
    {
        if (splinePath == null)
        {
            Debug.LogError("¡ERROR! El Spawner NO tiene asignado el Path_Bucle en el Inspector.");
            return;
        }

        StartCoroutine(SpawnWaveRoutine());
    }

    private IEnumerator SpawnWaveRoutine()
    {
        for (int i = 0; i < enemiesInWave; i++)
        {
            // 1. Calculamos el punto de inicio exacto de la curva en el mundo real
            Vector3 startPos = Vector3.zero;
            float3 localStart = splinePath.EvaluatePosition(0f);
            startPos = splinePath.transform.TransformPoint((Vector3)localStart);

            // RADAR DE DEPURACIÓN: Te avisa si están naciendo fuera del mapa
            if (startPos.y > 15f || startPos.y < -15f || startPos.x > 15f || startPos.x < -15f)
            {
                Debug.LogWarning("¡OJO! Un avión está naciendo en coordenadas extremas: " + startPos + ". Probablemente no lo veas en pantalla.");
            }

            // 2. Sacamos al enemigo del Pool
            GameObject enemyObj = ObjectPooler.Instance.SpawnFromPool(enemyPoolTag, startPos, Quaternion.identity);

            if (enemyObj != null)
            {
                EnemyMovementSpline movement = enemyObj.GetComponent<EnemyMovementSpline>();
                if (movement != null)
                {
                    movement.SetSplinePath(splinePath);
                }
            }

            yield return new WaitForSeconds(timeBetweenSpawns);
        }
    }
}