using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ScrollingBackground : MonoBehaviour
{
    public float velocidadFondo = 2f;
    private SpriteRenderer sr;
    private Vector2 tamanoOriginal;
    private float desplazamiento = 0f;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        // Guardamos el tamaño que le pusiste en el Inspector (20x30)
        tamanoOriginal = sr.size;
    }

    void Update()
    {
        // Calculamos cuánto se mueve
        desplazamiento += velocidadFondo * Time.deltaTime;

        // Modificamos internamente cómo se dibuja el mosaico
        // El tamaño se mantiene igual, solo "deslizamos" el dibujo por dentro
        sr.size = new Vector2(tamanoOriginal.x, tamanoOriginal.y + desplazamiento);
    }
}