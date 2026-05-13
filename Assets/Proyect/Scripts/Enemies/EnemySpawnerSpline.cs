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
    [SerializeField] private SplineContainer splinePath;

    [Header("Conexión con el Premio (Opcional)")]
    [SerializeField] private RedSquadronManager squadronManager;

    private void OnEnable()
    {
        StopAllCoroutines();

        if (splinePath == null)
        {
            Debug.LogError("[EnemySpawnerSpline] No tiene asignado el Spline Path.");
            return;
        }

        // Reseteamos el escuadrón antes de spawnear
        if (squadronManager != null)
            squadronManager.ResetSquadron(enemiesInWave);

        StartCoroutine(SpawnWaveRoutine());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator SpawnWaveRoutine()
    {
        yield return null;

        if (ObjectPooler.Instance == null)
        {
            Debug.LogError("[EnemySpawnerSpline] ObjectPooler no encontrado.");
            yield break;
        }

        for (int i = 0; i < enemiesInWave; i++)
        {
            // Posición inicial de la spline
            float3 localStart = splinePath.EvaluatePosition(0f);
            Vector3 startPos = splinePath.transform.TransformPoint((Vector3)localStart);
            startPos.z = 0;

            GameObject enemyObj = ObjectPooler.Instance.SpawnFromPool(
                enemyPoolTag, startPos, Quaternion.identity);

            if (enemyObj != null)
            {
                // Avisamos al LevelManager
                LevelManager.ReportarSpawn();

                // Asignamos la ruta
                EnemyMovementSpline movement = enemyObj.GetComponent<EnemyMovementSpline>();
                if (movement != null)
                    movement.SetSplinePath(splinePath);

                // Asignamos el escuadrón
                Enemy enemyScript = enemyObj.GetComponent<Enemy>();
                if (enemyScript != null)
                    enemyScript.mySquadron = squadronManager;
            }
            else
            {
                Debug.LogWarning("[EnemySpawnerSpline] Pool vacío para tag: " + enemyPoolTag);
            }

            yield return new WaitForSeconds(timeBetweenSpawns);
        }
    }
}