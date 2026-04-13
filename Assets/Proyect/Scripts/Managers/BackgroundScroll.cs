using UnityEngine;

public class BackgroundScroll : MonoBehaviour
{
    [Header("Configuración del Scroll")]
    [Tooltip("Velocidad a la que el fondo se mueve hacia abajo")]
    [SerializeField] private float scrollSpeed = 2f;

    private float spriteHeight;
    private Vector2 startPosition;

    private void Start()
    {
        // Guardamos la posición inicial donde colocamos el fondo
        startPosition = transform.position;

        // Obtenemos automáticamente el alto del sprite en unidades de Unity
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            spriteHeight = sr.bounds.size.y;
        }
        else
        {
            Debug.LogError("¡El fondo necesita un componente SpriteRenderer!");
        }
    }

    private void Update()
    {
        // Mathf.Repeat crea un ciclo que va de 0 a spriteHeight.
        // Cuando llega a spriteHeight, vuelve a 0.
        float newPositionY = Mathf.Repeat(Time.time * scrollSpeed, spriteHeight);

        // Movemos el fondo hacia abajo usando el valor cíclico
        transform.position = startPosition + Vector2.down * newPositionY;
    }
}