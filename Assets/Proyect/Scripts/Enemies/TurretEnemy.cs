using UnityEngine;

public class TurretEnemy : MonoBehaviour
{
    public Transform player;
    public string tagBala = "EnemyBullet";
    public float cadenciaDisparo = 2.5f;
    private float tiempoSiguienteDisparo;

    void Update()
    {
        // 1. BUSQUEDA: Si no lo tenemos, lo buscamos por Tag "Player"
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
            return;
        }

        // 2. ROTACIÓN: Mirar al jugador (Ajustado para sprites 2D)
        Vector3 direccion = player.position - transform.position;
        float angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angulo);

        // 3. DISPARO: Solo si está en pantalla (Ajustá el 6 según tu cámara)
        if (transform.position.y < 6f)
        {
            if (Time.time > tiempoSiguienteDisparo)
            {
                tiempoSiguienteDisparo = Time.time + cadenciaDisparo;
                ObjectPooler.Instance.SpawnFromPool(tagBala, transform.position, transform.rotation);
            }
        }
    }
}