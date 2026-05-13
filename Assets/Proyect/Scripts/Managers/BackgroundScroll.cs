using UnityEngine;

public class BackgroundScroll : MonoBehaviour
{
    [Header("Velocidad del scroll")]
    public float scrollSpeed = 2f;

    private float offsetY = 0f;
    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Acumulamos con deltaTime — nunca usa Time.time, nunca salta
        offsetY -= scrollSpeed * Time.deltaTime;

        // Si tiene SpriteRenderer Tiled (el mar), mueve la textura
        if (sr != null)
            sr.material.mainTextureOffset = new Vector2(0, offsetY);

        // Mueve el objeto completo hacia abajo (las nubes y naves)
        transform.Translate(Vector3.down * scrollSpeed * Time.deltaTime);
    }
}