using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    // Patrón Singleton para acceder fácilmente desde cualquier script
    public static ObjectPooler Instance { get; private set; }

    [System.Serializable]
    public class Pool
    {
        public string tag; // Identificador, ej: "PlayerBullet"
        public GameObject prefab;
        public int size; // Cantidad a pre-cargar
    }

    public List<Pool> pools;
    private Dictionary<string, Queue<GameObject>> poolDictionary;

    private void Awake()
    {
        // Configuración del Singleton
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Inicializamos el diccionario de las piscinas
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                obj.transform.SetParent(this.transform); // Lo emparentamos para mantener limpia la Jerarquía
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool con el tag " + tag + " no existe.");
            return null;
        }

        // Sacamos el primer objeto de la cola, lo activamos y lo posicionamos
        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        // Lo volvemos a poner al final de la cola para que se recicle en el futuro
        poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }
}