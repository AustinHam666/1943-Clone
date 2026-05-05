using UnityEngine;

public class BossMovement : MonoBehaviour
{
    [Header("Configuración de Entrada")]
    public float velocidadEntrada = 2f;
    public float stopY = 3.5f;

    [Header("Configuración de Combate")]
    public float velocidadLateral = 1.5f; // Bajale a 1.5 si es muy rápido
    public float amplitudHorizontal = 3.5f;

    private bool enPosicion = false;
    private float centroX;

    void Start()
    {
        // Guardamos el centro apenas empieza
        centroX = transform.position.x;
    }

    void Update()
    {
        if (!enPosicion)
        {
            // Baja suavemente hasta stopY
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, stopY), velocidadEntrada * Time.deltaTime);

            if (Mathf.Abs(transform.position.y - stopY) < 0.05f)
            {
                enPosicion = true;
                // Al llegar a posición, reseteamos el centroX por las dudas
                centroX = transform.position.x;
            }
        }
        else
        {
            // Movimiento lateral suave (Sinusoidal)
            float offset = Mathf.Sin(Time.time * velocidadLateral) * amplitudHorizontal;
            transform.position = new Vector3(centroX + offset, transform.position.y, transform.position.z);
        }
    }
}