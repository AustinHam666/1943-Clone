using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ScrollingBackground : MonoBehaviour
{
    public float velocidadFondo = 1f;

    private SpriteRenderer sr;
    private float offsetY = 0f;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        offsetY -= velocidadFondo * Time.deltaTime;
        sr.material.mainTextureOffset = new Vector2(0, offsetY);
    }
}