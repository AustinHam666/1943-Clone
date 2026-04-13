using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Configuración de la Oleada")]
    [SerializeField] private string enemyPoolTag = "EnemyZako";
    [SerializeField] private int enemiesInWave = 5;
    [SerializeField] private float timeBetweenSpawns = 0.5f;
    [SerializeField] private Transform pathContainer;

    private Transform[] wavePath;

    // Usamos Awake en lugar de Start para inicializar la ruta silenciosamente, 
    // sin disparar la oleada todavía.
    private void Awake()
    {
        if (pathContainer != null)
        {
            wavePath = new Transform[pathContainer.childCount];
            for (int i = 0; i < pathContainer.childCount; i++)
            {
                wavePath[i] = pathContainer.GetChild(i);
            }
        }
    }

    /// <summary>
    /// El LevelManager (Director de Nivel) llamará a este método 
    /// para iniciar la oleada en el segundo exacto que corresponda.
    /// </summary>
    public void TriggerWave()
    {
        StartCoroutine(SpawnWaveRoutine());
    }

    private IEnumerator SpawnWaveRoutine()
    {
        for (int i = 0; i < enemiesInWave; i++)
        {
            // 1. Pedimos un enemigo al Pooler
            GameObject enemyObj = ObjectPooler.Instance.SpawnFromPool(enemyPoolTag, Vector3.zero, Quaternion.identity);

            if (enemyObj != null)
            {
                // 2. Le asignamos la ruta al script de movimiento
                EnemyMovement movement = enemyObj.GetComponent<EnemyMovement>();
                if (movement != null)
                {
                    movement.SetPath(wavePath);
                }
            }

            // 3. Esperamos el tiempo definido antes de sacar al siguiente avión
            yield return new WaitForSeconds(timeBetweenSpawns);
        }
    }
}