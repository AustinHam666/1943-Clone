using UnityEngine;

public class MiniPlane : MonoBehaviour
{
    [Header("Persecución")]
    public float velocidad = 4f;
    public float velocidadRotacion = 150f; // Qué tan rápido dobla para seguirte
    private Transform player;

    [Header("Disparo")]
    public string tagBalaEnemiga = "EnemyBullet"; // Usamos el Tag exacto de tu GameManager
    public float tiempoEntreDisparos = 2f;
    private float proximoDisparo;

    [Header("Límites de la Pantalla (Pared Invisible)")]
    public float limiteMinX = -8f;
    public float limiteMaxX = 8f;
    public float limiteMinY = -4.5f;
    public float limiteMaxY = 4.5f;

    void OnEnable()
    {
        // OnEnable se ejecuta cada vez que el avioncito "nace" del Object Pooler
        // Buscamos al jugador por su Tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        // Le damos un segundo de gracia antes de empezar a disparar
        proximoDisparo = Time.time + 1f;
    }

    void Update()
    {
        // 1. PERSECUCIÓN: Apuntar hacia el jugador
        if (player != null)
        {
            // Calculamos el vector de dirección hacia el jugador
            Vector3 direccionAlJugador = player.position - transform.position;
            direccionAlJugador.Normalize();

            // Calculamos el ángulo. Restamos 90f porque tu sprite original apunta hacia arriba.
            float angulo = Mathf.Atan2(direccionAlJugador.y, direccionAlJugador.x) * Mathf.Rad2Deg - 90f;
            Quaternion rotacionDeseada = Quaternion.Euler(0, 0, angulo);

            // Giramos el avión suavemente hacia ese ángulo
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotacionDeseada, velocidadRotacion * Time.deltaTime);
        }

        // Avanzar siempre hacia su propio "Adelante"
        transform.Translate(Vector3.up * velocidad * Time.deltaTime);

        // 2. LÍMITES: No salirse del mapa
        // Mathf.Clamp encierra el valor entre un mínimo y un máximo. Si trata de pasarse, lo frena.
        float posX = Mathf.Clamp(transform.position.x, limiteMinX, limiteMaxX);
        float posY = Mathf.Clamp(transform.position.y, limiteMinY, limiteMaxY);
        transform.position = new Vector3(posX, posY, transform.position.z);

        // 3. DISPARO
        if (Time.time > proximoDisparo)
        {
            proximoDisparo = Time.time + tiempoEntreDisparos;
            Disparar();
        }
    }

    void Disparar()
    {
        // Pedimos una bala enemiga al Pool y la instanciamos con la misma rotación del avioncito
        ObjectPooler.Instance.SpawnFromPool(tagBalaEnemiga, transform.position, transform.rotation);
    }
}