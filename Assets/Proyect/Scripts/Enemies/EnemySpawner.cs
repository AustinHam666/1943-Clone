using System.Collections;
using UnityEngine;
using UnityEngine.Splines;

public class EnemySpawner : MonoBehaviour
{
    public enum TipoMovimiento
    {
        BajarRecto,
        RutaWaypoints,
        RutaSpline,
        ZakoLoop,
        Nodriza
    }

    public enum TipoFormacion
    {
        Linea,
        Diagonal
    }

    [Header("Configuración del enemigo")]
    public string enemyPoolTag = "EnemyZako";
    public int cantidadEnemigos = 5;
    public float tiempoEntreSpawns = 0.3f;
    public TipoMovimiento tipoMovimiento = TipoMovimiento.ZakoLoop;

    [Header("Posición de spawn")]
    public Transform puntoDeSpawn;

    [Header("Formación")]
    public TipoFormacion tipoFormacion = TipoFormacion.Linea;
    public float offsetX = -0.8f;
    public float offsetY = 0.4f;

    [Header("Ruta (solo para RutaWaypoints)")]
    public Transform pathContainer;

    [Header("Spline (solo para RutaSpline)")]
    public SplineContainer splinePath;

    [Header("Escuadrón (solo para ZakoRed)")]
    public RedSquadronManager squadronManager;

    private Transform[] wavePath;

    private void Awake()
    {
        if (pathContainer != null && pathContainer.childCount > 0)
        {
            wavePath = new Transform[pathContainer.childCount];
            for (int i = 0; i < pathContainer.childCount; i++)
                wavePath[i] = pathContainer.GetChild(i);
        }
    }

    private void OnEnable()
    {
        StopAllCoroutines();
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
            Debug.LogError("[EnemySpawner] ObjectPooler no encontrado.");
            yield break;
        }

        // Reseteamos el escuadrón ANTES de spawnear para que cuente bien
        if (squadronManager != null)
            squadronManager.ResetSquadron(cantidadEnemigos);

        Vector3 basePos = puntoDeSpawn != null
            ? puntoDeSpawn.position
            : transform.position;

        for (int i = 0; i < cantidadEnemigos; i++)
        {
            Vector3 spawnPos = basePos;

            if (tipoFormacion == TipoFormacion.Diagonal)
                spawnPos = basePos + new Vector3(offsetX * i, offsetY * i, 0);

            spawnPos.z = 0;

            GameObject enemyObj = ObjectPooler.Instance.SpawnFromPool(
                enemyPoolTag, spawnPos, Quaternion.identity);

            if (enemyObj != null)
            {
                LevelManager.ReportarSpawn();
                ConfigurarMovimiento(enemyObj, spawnPos);

                if (squadronManager != null)
                {
                    Enemy enemy = enemyObj.GetComponent<Enemy>();
                    if (enemy != null)
                        enemy.mySquadron = squadronManager;
                }
            }
            else
            {
                Debug.LogWarning("[EnemySpawner] Pool vacío para tag: " + enemyPoolTag);
            }

            yield return new WaitForSeconds(tiempoEntreSpawns);
        }
    }

    private void ConfigurarMovimiento(GameObject enemyObj, Vector3 spawnPos)
    {
        switch (tipoMovimiento)
        {
            case TipoMovimiento.RutaWaypoints:
                EnemyMovement movement = enemyObj.GetComponent<EnemyMovement>();
                if (movement != null && wavePath != null)
                    movement.SetPath(wavePath);
                break;

            case TipoMovimiento.RutaSpline:
                EnemyMovementSpline splineMovement = enemyObj.GetComponent<EnemyMovementSpline>();
                if (splineMovement != null && splinePath != null)
                    splineMovement.SetSplinePath(splinePath);
                break;

            case TipoMovimiento.ZakoLoop:
            case TipoMovimiento.BajarRecto:
            case TipoMovimiento.Nodriza:
                break;
        }
    }
}