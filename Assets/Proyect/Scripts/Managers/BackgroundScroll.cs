using UnityEngine;

public class BackgroundScroll : MonoBehaviour
{
    [Header("Configuración del Scroll")]
    [SerializeField] private float scrollSpeed = 2f;

    // Cambiamos a [SerializeField] para que lo escribas tú en el Inspector
    [Tooltip("La altura total de tu mapa pintado (Ej: 20 o 30)")]
    [SerializeField] private float mapHeight = 20f;

    private Vector2 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        // Usamos el mapHeight que definiste a mano
        float newPositionY = Mathf.Repeat(Time.time * scrollSpeed, mapHeight);
        transform.position = startPosition + Vector2.down * newPositionY;
    }
}