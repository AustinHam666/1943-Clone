using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Oleada
{
    public string nombreOleada;
    public float tiempoDeAparicion; // En qué segundo del juego aparece
    public string tagEnemigo;       // El nombre que pusiste en el Object Pooler (ej: "EnemyZako")
    public Vector3 posicionSpawn;   // Ej: (0, 6, 0) para que aparezca arriba
}

public class LevelSpawner : MonoBehaviour
{
    public List<Oleada> cronogramaDeNivel;
    private float cronometro = 0f;
    private int indiceOleada = 0;
    public bool nivelFinalizado = false;

    void Update()
    {
        if (nivelFinalizado) return;

        cronometro += Time.deltaTime;

        // Si hay oleadas pendientes y llegamos al tiempo de la siguiente
        if (indiceOleada < cronogramaDeNivel.Count && cronometro >= cronogramaDeNivel[indiceOleada].tiempoDeAparicion)
        {
            SpawnEnemigo(cronogramaDeNivel[indiceOleada]);
            indiceOleada++;
        }
    }

    void SpawnEnemigo(Oleada ola)
    {
        // Usamos tu Object Pooler para que sea eficiente
        ObjectPooler.Instance.SpawnFromPool(ola.tagEnemigo, ola.posicionSpawn, Quaternion.identity);
    }

    // Método para cuando lleguemos al Boss
    public void FrenarNivelParaBoss()
    {
        nivelFinalizado = true;
    }
}