using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class ItemPow : MonoBehaviour
{
    // Añadimos el nuevo tipo "Auto" (Metralleta)
    public enum PowType { Energy, Shotgun, Auto }

    [Header("Configuración")]
    [SerializeField] private float fallSpeed = 2f;
    [SerializeField] private float energyRestoreAmount = 20f;
    [Tooltip("Cantidad de disparos necesarios para cambiar el ítem")]
    [SerializeField] private int hitsToChange = 3;

    private PowType currentType = PowType.Energy;
    private int currentHits = 0; // Contador interno de disparos

    private SpriteRenderer sr;
    private Rigidbody2D rb;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    private void OnEnable()
    {
        currentType = PowType.Energy;
        currentHits = 0; // Reiniciamos los golpes al nacer
        UpdateVisuals();
    }

    private void FixedUpdate()
    {
        rb.velocity = Vector2.down * fallSpeed;
    }

    public void TakeHit()
    {
        currentHits++; // Sumamos un impacto

        // Si llegamos a la cantidad necesaria de tiros, cambiamos de forma
        if (currentHits >= hitsToChange)
        {
            currentHits = 0; // Reseteamos el contador

            // Ciclo de armas: Energía -> Escopeta -> Auto -> (Vuelve a empezar)
            if (currentType == PowType.Energy) currentType = PowType.Shotgun;
            else if (currentType == PowType.Shotgun) currentType = PowType.Auto;
            else currentType = PowType.Energy;

            UpdateVisuals();
        }
    }

    private void UpdateVisuals()
    {
        if (currentType == PowType.Energy) sr.color = Color.green;
        else if (currentType == PowType.Shotgun) sr.color = Color.yellow;
        else if (currentType == PowType.Auto) sr.color = Color.cyan; // Celeste para la metralleta
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                if (currentType == PowType.Energy) player.RestoreEnergy(energyRestoreAmount);
                else if (currentType == PowType.Shotgun) player.EquipShotgun();
                else if (currentType == PowType.Auto) player.EquipAuto();

                gameObject.SetActive(false);
            }
        }
    }
}