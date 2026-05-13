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

    [Header("Sprites de Movimiento")]
    public SpriteRenderer sRenderer;
    public Sprite spriteIzquierda;
    public Sprite spriteCentro;
    public Sprite spriteDerecha;
    public float umbralGiro = 0.01f;

    private float angulo = 0;
    private float vueltasCompletadas = 0;
    // FIX CRÍTICO: usamos un timer propio en vez de Time.time
    private float timerSpawn = 0f;
    private Vector2 posicionAnterior;

    private void OnEnable()
    {
        // Reseteo completo al salir del pool
        estadoActual = EstadoNodriza.Entrando;
        angulo = 0f;
        vueltasCompletadas = 0f;
        // Empezamos el timer en el tiempo entre hijos para que
        // no spawnee instantáneamente al aparecer
        timerSpawn = tiempoEntreHijos;
        posicionAnterior = transform.position;

        if (sRenderer == null) sRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
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

        float movimientoH = transform.position.x - posicionAnterior.x;
        ActualizarSprite(movimientoH);
        posicionAnterior = transform.position;

        LógicaSpawneo();
    }

    void ActualizarSprite(float deltaX)
    {
        if (sRenderer == null) return;
        if (deltaX > umbralGiro * Time.deltaTime)
            sRenderer.sprite = spriteDerecha;
        else if (deltaX < -umbralGiro * Time.deltaTime)
            sRenderer.sprite = spriteIzquierda;
        else
            sRenderer.sprite = spriteCentro;
    }

    void MoverHaciaElCentro()
    {
        Vector3 puntoInicioGiro = puntoCentral + new Vector3(radioGiro, 0, 0);
        transform.position = Vector3.MoveTowards(
            transform.position, puntoInicioGiro, velocidadEntrada * Time.deltaTime);

        if (Vector3.Distance(transform.position, puntoInicioGiro) < 0.1f)
        {
            estadoActual = EstadoNodriza.Girando;
            angulo = 0f;
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
            if (vueltasCompletadas >= vueltasADar)
                estadoActual = EstadoNodriza.Estatico;
        }
    }

    void Flotar()
    {
        float balanceoY = puntoCentral.y + Mathf.Sin(Time.time * 2f) * 0.3f;
        transform.position = new Vector3(puntoCentral.x, balanceoY, 0);
    }

    void LógicaSpawneo()
    {
        // FIX: usamos un timer propio que cuenta desde 0, no Time.time global
        timerSpawn -= Time.deltaTime;
        if (timerSpawn <= 0f)
        {
            timerSpawn = tiempoEntreHijos;

            GameObject nuevoAvion = ObjectPooler.Instance.SpawnFromPool(
                tagMiniCaza, transform.position, Quaternion.Euler(0, 0, 180f));

            if (nuevoAvion == null)
                Debug.LogError("Pool vacío o tag incorrecto: " + tagMiniCaza);
        }
    }
}