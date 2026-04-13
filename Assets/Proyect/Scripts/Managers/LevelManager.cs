using System.Collections;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Oleadas del Nivel 1")]
    [Tooltip("Arrastra aquí tus objetos Spawner de la jerarquía")]
    [SerializeField] private EnemySpawner wave1_UShape;
    [SerializeField] private EnemySpawner wave2_RedSquadron;

    // [SerializeField] private GameObject bossPrefab; // Lo usaremos luego para Rikaku

    private void Start()
    {
        // Al iniciar la escena, arranca el reloj del nivel
        StartCoroutine(LevelTimelineRoutine());
    }

    private IEnumerator LevelTimelineRoutine()
    {
        Debug.Log("Misión 1: ¡Batalla de Midway Iniciada!");

        // Esperamos 3 segundos al empezar para que el jugador se acomode
        yield return new WaitForSeconds(3f);

        Debug.Log("0:03 - Entra la primera formación (Oleada 1)");
        if (wave1_UShape != null) wave1_UShape.TriggerWave();

        // Esperamos 8 segundos más
        yield return new WaitForSeconds(8f);

        Debug.Log("0:11 - Entra el escuadrón rojo (Oleada 2)");
        if (wave2_RedSquadron != null) wave2_RedSquadron.TriggerWave();

        // Esperamos 15 segundos de combate
        yield return new WaitForSeconds(15f);

        Debug.Log("0:26 - ¡WARNING! El Jefe Rikaku se acerca...");
        // TODO: Activar el BossSpawner o instanciar a Rikaku
    }
}