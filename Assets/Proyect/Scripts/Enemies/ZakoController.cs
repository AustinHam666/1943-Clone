using UnityEngine;

public class ZakoController : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private Sprite normalSprite;       // El que vuela recto
    [SerializeField] private Sprite[] turnFrames;       // Acá arrastrás tus frames de giro
    [SerializeField] private float frameRate = 0.1f;    // Velocidad del giro

    [Header("Configuración")]
    [SerializeField] private float speed = 4f;
    [SerializeField] private float yTurnThreshold = -2f;

    private SpriteRenderer sr;
    private int frameIndex = 0;
    private float timer = 0f;
    private bool isTurning = false;
    private bool finishedTurn = false;

    void Awake() { sr = GetComponent<SpriteRenderer>(); }

    void Update()
    {
        // 1. Movimiento básico (bajando)
        if (!isTurning && !finishedTurn)
        {
            transform.Translate(Vector3.down * speed * Time.deltaTime);
            if (transform.position.y <= yTurnThreshold) isTurning = true;
        }
        // 2. Lógica de Giro (Cambiando el sprite por código)
        else if (isTurning)
        {
            timer += Time.deltaTime;
            if (timer >= frameRate)
            {
                timer = 0;
                sr.sprite = turnFrames[frameIndex];
                frameIndex++;

                if (frameIndex >= turnFrames.Length)
                {
                    isTurning = false;
                    finishedTurn = true;
                    // Ya terminó el giro, volvemos al sprite normal o uno apuntando arriba
                    sr.sprite = normalSprite;
                    transform.rotation = Quaternion.Euler(0, 0, 0); // Apuntando arriba
                }
            }
        }
        // 3. Subiendo para escapar
        else if (finishedTurn)
        {
            transform.Translate(Vector3.up * speed * Time.deltaTime);
            if (transform.position.y > 10f) Destroy(gameObject);
        }
    }
}