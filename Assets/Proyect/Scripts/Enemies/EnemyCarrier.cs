using UnityEngine;

public class EnemyCarrier : MonoBehaviour
{
    public enum EstadoNodriza { Entrando, Girando, Estatico }
    public EstadoNodriza estadoActual = EstadoNodriza.Entrando;

    [Header("Configuración de Movimiento")]
    public Vector3 puntoCentral = new Vector3(0, 3, 0);
    public float velocidadEntrada = 3f;
    public float radioGiro = 2f;
    public float velocidadGiro = 2f;
    public int vueltasADar = 2;

    [Header("Spawneo")]
    public string tagMiniCaza = "MiniCaza";
    public float tiempoEntreHijos = 1.5f;

    [Header("Sprites de Movimiento (Manual)")]
    public SpriteRenderer sRenderer;
    public Sprite spriteIzquierda;
    public Sprite spriteCentro;
    public Sprite spriteDerecha;
    public float umbralGiro = 0.01f;

    private float angulo = 0;
    private float vueltasCompletadas = 0;
    private float tiempoSiguienteHijo;
    private Vector2 posicionAnterior;

    void Start()
    {
        if (sRenderer == null) sRenderer = GetComponent<SpriteRenderer>();
        posicionAnterior = transform.position;
    }

    void Update()
    {
        float movimientoH = 0;

        switch (estadoActual)
        {
            case EstadoNodriza.Entrando:
                MoverHaciaElCentro();
                break;
            case EstadoNodriza.Girando:
                DarVueltas();
                break;
            case EstadoNodriza.Estatico:
                Flotar();
                break;
        }

        // Detectamos el movimiento lateral comparando con el frame anterior
        movimientoH = transform.position.x - posicionAnterior.x;
        ActualizarSprite(movimientoH);

        posicionAnterior = transform.position;
        LógicaSpawneo();
    }

    void ActualizarSprite(float deltaX)
    {
        if (sRenderer == null) return;

        // Movimiento a la derecha
        if (deltaX > umbralGiro * Time.deltaTime)
            sRenderer.sprite = spriteDerecha;
        // Movimiento a la izquierda
        else if (deltaX < -umbralGiro * Time.deltaTime)
            sRenderer.sprite = spriteIzquierda;
        // Casi quieto
        else
            sRenderer.sprite = spriteCentro;
    }

    void MoverHaciaElCentro()
    {
        // Calculamos el punto exacto donde empieza el círculo en base al radio
        Vector3 puntoInicioGiro = puntoCentral + new Vector3(radioGiro, 0, 0);

        transform.position = Vector3.MoveTowards(transform.position, puntoInicioGiro, velocidadEntrada * Time.deltaTime);

        if (Vector3.Distance(transform.position, puntoInicioGiro) < 0.1f)
        {
            estadoActual = EstadoNodriza.Girando;
            angulo = 0f; // Empezamos el círculo exactamente desde este borde
        }
    }

    void DarVueltas()
    {
        angulo += velocidadGiro * Time.deltaTime;
        float x = puntoCentral.x + Mathf.Cos(angulo) * radioGiro;
        float y = puntoCentral.y + Mathf.Sin(angulo) * radioGiro;
        transform.position = new Vector3(x, y, 0);

        if (angulo >= Mathf.PI * 2)
        {
            angulo = 0;
            vueltasCompletadas++;
            if (vueltasCompletadas >= vueltasADar) estadoActual = EstadoNodriza.Estatico;
        }
    }

    void Flotar()
    {
        float balanceoY = puntoCentral.y + Mathf.Sin(Time.time * 2f) * 0.3f;
        transform.position = new Vector3(puntoCentral.x, balanceoY, 0);
    }

    void LógicaSpawneo()
    {
        if (Time.time > tiempoSiguienteHijo)
        {
            tiempoSiguienteHijo = Time.time + tiempoEntreHijos;

            // Le pedimos al Pooler que suelte el avioncito
            GameObject nuevoAvion = ObjectPooler.Instance.SpawnFromPool(tagMiniCaza, transform.position, Quaternion.Euler(0, 0, 180f));

            // El chismoso para la consola:
            if (nuevoAvion != null)
            {
                Debug.Log("¡Avioncito soltado con éxito en la posición: " + transform.position + "!");
            }
            else
            {
                Debug.LogError("¡PELIGRO! El Pooler devolvió NULL. O el Tag '" + tagMiniCaza + "' está mal escrito, o la cola del Pool está vacía.");
            }
        }
    }
}