using UnityEngine;

public class MiniPlane : MonoBehaviour
{
    [Header("Persecución")]
    public float velocidad = 4f;
    public float velocidadRotacion = 150f;

    [Header("Disparo")]
    public string tagBalaEnemiga = "EnemyBullet";
    public float tiempoEntreDisparos = 2f;

    [Header("Límites de la Pantalla")]
    public float limiteMinX = -8f;
    public float limiteMaxX = 8f;
    public float limiteMinY = -4.5f;
    public float limiteMaxY = 4.5f;

    private Transform player;
    // FIX: timer propio en vez de Time.time
    private float timerDisparo = 0f;

    void OnEnable()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        // Gracia inicial antes de disparar
        timerDisparo = -1f;
    }

    void Update()
    {
        if (player != null)
        {
            Vector3 direccionAlJugador = (player.position - transform.position).normalized;
            float angulo = Mathf.Atan2(direccionAlJugador.y, direccionAlJugador.x) * Mathf.Rad2Deg - 90f;
            Quaternion rotacionDeseada = Quaternion.Euler(0, 0, angulo);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, rotacionDeseada, velocidadRotacion * Time.deltaTime);
        }

        transform.Translate(Vector3.up * velocidad * Time.deltaTime);

        float posX = Mathf.Clamp(transform.position.x, limiteMinX, limiteMaxX);
        float posY = Mathf.Clamp(transform.position.y, limiteMinY, limiteMaxY);
        transform.position = new Vector3(posX, posY, 0);

        // FIX: timer propio
        timerDisparo += Time.deltaTime;
        if (timerDisparo >= tiempoEntreDisparos)
        {
            timerDisparo = 0f;
            Disparar();
        }
    }

    void Disparar()
    {
        if (ObjectPooler.Instance != null)
            ObjectPooler.Instance.SpawnFromPool(tagBalaEnemiga, transform.position, transform.rotation);
    }
}