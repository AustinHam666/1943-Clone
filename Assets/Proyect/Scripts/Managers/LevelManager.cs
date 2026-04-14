using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [System.Serializable]
    public class WaveEvent
    {
        public string waveName;      // Nombre para organizarte (ej: "Oleada Bucle 1")
        public float spawnTime;      // Segundo exacto en que aparece
        public GameObject spawner;   // El objeto Spawner (el que tiene el script de la wave)
    }

    [Header("Configuración del Nivel 1943")]
    [SerializeField] private List<WaveEvent> levelWaves;

    private void Start()
    {
        // Al empezar el nivel, iniciamos el cronómetro de las oleadas
        StartCoroutine(LevelTimelineRoutine());
    }

    private IEnumerator LevelTimelineRoutine()
    {
        float currentTime = 0f;

        // Recorremos la lista de oleadas que configuraste en el Inspector
        foreach (WaveEvent wave in levelWaves)
        {
            // Esperamos hasta que llegue el momento de esta oleada
            while (currentTime < wave.spawnTime)
            {
                currentTime += Time.deltaTime;
                yield return null;
            }

            // LANZAR LA OLEADA:
            if (wave.spawner != null)
            {
                // Este es el cambio clave: 
                // Busca cualquier script que tenga el método "TriggerWave" y lo ejecuta.
                // Así funciona tanto para el Spawner viejo como para el de Splines.
                wave.spawner.SendMessage("TriggerWave", SendMessageOptions.DontRequireReceiver);

                Debug.Log("Lanzando oleada: " + wave.waveName + " en el segundo " + currentTime);
            }
        }

        // Aquí podrías añadir un aviso de que el nivel terminó o que viene el Jefe Rikaku
        Debug.Log("Todas las oleadas lanzadas. ¡Prepárate para el jefe!");
    }
}