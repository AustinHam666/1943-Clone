using UnityEngine;

public class ZakoController : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private Sprite normalSprite;      // zakoAssets_0 (bajando)
    [SerializeField] private Sprite[] turnFrames;      // zakoAssets_1, 2, 3 (giro)
    [SerializeField] private Sprite upSprite;          // zakoAssets_4 (subiendo)
    [SerializeField] private float frameRate = 0.1f;

    [Header("Configuración")]
    [SerializeField] private float speed = 4f;
    [SerializeField] private float yTurnThreshold = -2f;

    private SpriteRenderer sr;
    private int frameIndex = 0;
    private float timer = 0f;
    private bool isTurning = false;
    private bool finishedTurn = false;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        frameIndex = 0;
        timer = 0f;
        isTurning = false;
        finishedTurn = false;
        if (sr == null) sr = GetComponent<SpriteRenderer>();
        if (normalSprite != null) sr.sprite = normalSprite;
        transform.rotation = Quaternion.identity;
    }

    void Update()
    {
        if (!isTurning && !finishedTurn)
        {
            transform.Translate(Vector3.down * speed * Time.deltaTime);
            if (transform.position.y <= yTurnThreshold)
                isTurning = true;
        }
        else if (isTurning)
        {
            timer += Time.deltaTime;
            if (timer >= frameRate)
            {
                timer = 0;
                if (turnFrames != null && turnFrames.Length > 0 && frameIndex < turnFrames.Length)
                    sr.sprite = turnFrames[frameIndex];
                frameIndex++;
                if (frameIndex >= turnFrames.Length)
                {
                    isTurning = false;
                    finishedTurn = true;
                    // Al terminar el giro ponemos el sprite de subida
                    if (upSprite != null) sr.sprite = upSprite;
                    transform.rotation = Quaternion.identity;
                }
            }
        }
        else if (finishedTurn)
        {
            transform.Translate(Vector3.up * speed * Time.deltaTime);
            if (transform.position.y > 10f)
                gameObject.SetActive(false);
        }
    }
}