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

    public void TriggerWave()
    {
        StartCoroutine(SpawnWaveRoutine());
    }

    private IEnumerator SpawnWaveRoutine()
    {
        for (int i = 0; i < enemiesInWave; i++)
        {
            // 1. DETERMINAR POSICIÓN: Si tenemos ruta, usamos el primer punto. Si no, usamos la posición del Spawner
            Vector3 spawnPos = (wavePath != null && wavePath.Length > 0) ? wavePath[0].position : transform.position;

            // 2. PEDIR AL POOL: Usamos spawnPos en lugar de Vector3.zero
            GameObject enemyObj = ObjectPooler.Instance.SpawnFromPool(enemyPoolTag, spawnPos, Quaternion.identity);

            if (enemyObj != null)
            {
                // 3. CONFIGURAR SEGÚN EL TIPO:

                // Si es un avión que sigue ruta (EnemyMovement)
                EnemyMovement movement = enemyObj.GetComponent<EnemyMovement>();
                if (movement != null)
                {
                    movement.SetPath(wavePath);
                }

                // Si es un Zako (Acróbata)
                ZakoController zako = enemyObj.GetComponent<ZakoController>();
                if (zako != null)
                {
                    // No necesita SetPath, pero podés re-inicializarlo si hace falta
                    // O dejar que su propio script tome el control.
                }
            }

            yield return new WaitForSeconds(timeBetweenSpawns);
        }
    }
}