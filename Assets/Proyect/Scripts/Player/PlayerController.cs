using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float padding = 0.5f;

    [Header("Sprites del Pucará")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite rectoSprite;
    [SerializeField] private Sprite inclinadoSprite;

    [Header("Efecto de Daño")]
    [SerializeField] private Sprite damageFlashSprite; // Acá va el Pucará todo blanco (image_65.png)
    [SerializeField] private float flashDuration = 0.1f;
    [SerializeField] private int flashCount = 4;
    [SerializeField] private float immunityDurationAfterDamage = 1.0f;

    [Header("Sistema de Energía")]
    [SerializeField] private float maxEnergy = 100f;
    [SerializeField] private float passiveDrainRate = 1.5f;

    [Header("Sistema de Disparo")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.15f;
    [SerializeField] private float shotgunSpreadAngle = 15f;
    [SerializeField] private float powerUpDuration = 10f;

    [Header("Mecánica de Evasión (Loop)")]
    [SerializeField] private int maxEvades = 3;
    [SerializeField] private float evadeDuration = 1.5f;

    // Variables internas
    private float currentEnergy;
    private int currentEvades;
    private Vector2 moveInput;
    private Rigidbody2D rb;
    private Collider2D col;
    private Vector2 minBounds;
    private Vector2 maxBounds;

    // Estados y Control de Armas
    private bool isFiring;
    private float fireTimer;
    private bool isEvading;
    private bool hasShotgun = false;
    private bool hasAuto = false;
    private float baseFireRate;
    private Coroutine weaponTimerRoutine;

    // Variables Daño
    private bool isDamagedAndImmune = false;
    private Coroutine damageFlashRoutine;

    public float CurrentEnergy => currentEnergy;
    public int CurrentEvades => currentEvades;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;

        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        currentEnergy = maxEnergy;
        currentEvades = maxEvades;
        baseFireRate = fireRate;
        CalculateScreenBounds();

        spriteRenderer.sprite = rectoSprite;
        spriteRenderer.flipX = false;
        spriteRenderer.color = Color.white;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateEnergyUI(currentEnergy, maxEnergy);
        }
    }

    private void Update()
    {
        if (isEvading) return;

        HandleEnergyDrain();
        HandleShooting();
        HandleSpriteIncline();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    #region Input System Callbacks

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnFire(InputValue value)
    {
        isFiring = value.isPressed;
    }

    public void OnEvade(InputValue value)
    {
        if (value.isPressed && !isEvading && currentEvades > 0)
        {
            StartCoroutine(PerformEvadeRoutine());
        }
    }

    public void OnPause(InputValue value)
    {
        if (value.isPressed && GameManager.Instance != null)
        {
            GameManager.Instance.TogglePause();
        }
    }

    #endregion

    #region Lógica de Animación de Sprites

    private void HandleSpriteIncline()
    {
        // Si estamos en medio del parpadeo de daño, no cambiamos el sprite por la inclinación
        if (isDamagedAndImmune) return;

        if (moveInput.x > 0.1f)
        {
            spriteRenderer.sprite = inclinadoSprite;
            spriteRenderer.flipX = false;
        }
        else if (moveInput.x < -0.1f)
        {
            spriteRenderer.sprite = inclinadoSprite;
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.sprite = rectoSprite;
            spriteRenderer.flipX = false;
        }
    }

    #endregion

    #region Lógica de Parpadeo de Daño y Capas

    private IEnumerator PerformDamageFlashRoutine()
    {
        isDamagedAndImmune = true;

        // Buscamos las capas de físicas
        int playerLayer = LayerMask.NameToLayer("Player");
        int immuneLayer = LayerMask.NameToLayer("ImmunePlayer");

        // Pasamos a la capa inmune (para que las balas te pasen de largo)
        if (immuneLayer != -1) gameObject.layer = immuneLayer;

        for (int i = 0; i < flashCount; i++)
        {
            // Ponemos el sprite blanco
            spriteRenderer.sprite = damageFlashSprite;
            yield return new WaitForSeconds(flashDuration);

            // Volvemos al sprite normal
            spriteRenderer.sprite = rectoSprite;
            yield return new WaitForSeconds(flashDuration);
        }

        // Nos aseguramos que quede el sprite normal al final
        spriteRenderer.sprite = rectoSprite;

        // Esperamos el tiempo de gracia restante
        yield return new WaitForSeconds(immunityDurationAfterDamage - (flashCount * flashDuration * 2));

        // Volvemos a la capa física normal
        if (playerLayer != -1) gameObject.layer = playerLayer;
        isDamagedAndImmune = false;
    }

    #endregion

    #region Lógica de Evasión (Loop)

    private IEnumerator PerformEvadeRoutine()
    {
        isEvading = true;
        currentEvades--;

        col.enabled = false;

        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale * 1.5f;

        float timer = 0f;
        float halfDuration = evadeDuration / 2f;

        while (timer < halfDuration)
        {
            transform.localScale = Vector3.Lerp(originalScale, targetScale, timer / halfDuration);
            timer += Time.deltaTime;
            yield return null;
        }

        timer = 0f;

        while (timer < halfDuration)
        {
            transform.localScale = Vector3.Lerp(targetScale, originalScale, timer / halfDuration);
            timer += Time.deltaTime;
            yield return null;
        }

        transform.localScale = originalScale;
        col.enabled = true;
        isEvading = false;
    }

    #endregion

    #region Lógica de Movimiento y Disparo

    private void MovePlayer()
    {
        if (moveInput == Vector2.zero) return;
        Vector2 newPosition = rb.position + moveInput * moveSpeed * Time.fixedDeltaTime;
        newPosition.x = Mathf.Clamp(newPosition.x, minBounds.x + padding, maxBounds.x - padding);
        newPosition.y = Mathf.Clamp(newPosition.y, minBounds.y + padding, maxBounds.y - padding);
        rb.MovePosition(newPosition);
    }

    private void HandleShooting()
    {
        if (isFiring && fireTimer <= 0f)
        {
            FireWeapon();
            fireTimer = fireRate;
        }
        if (fireTimer > 0f) fireTimer -= Time.deltaTime;
    }

    private void FireWeapon()
    {
        if (ObjectPooler.Instance != null && firePoint != null)
        {
            ObjectPooler.Instance.SpawnFromPool("PlayerBullet", firePoint.position, firePoint.rotation);

            if (hasShotgun)
            {
                Quaternion leftAngle = Quaternion.Euler(0, 0, firePoint.eulerAngles.z + shotgunSpreadAngle);
                Quaternion rightAngle = Quaternion.Euler(0, 0, firePoint.eulerAngles.z - shotgunSpreadAngle);

                ObjectPooler.Instance.SpawnFromPool("PlayerBullet", firePoint.position, leftAngle);
                ObjectPooler.Instance.SpawnFromPool("PlayerBullet", firePoint.position, rightAngle);
            }
        }
    }

    #endregion

    #region Lógica de Energía y Vida

    private void HandleEnergyDrain()
    {
        if (currentEnergy > 0)
        {
            currentEnergy -= passiveDrainRate * Time.deltaTime;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.UpdateEnergyUI(currentEnergy, maxEnergy);
            }

            if (currentEnergy <= 0) Die();
        }
    }

    public void TakeDamage(float damageAmount)
    {
        if (isEvading || isDamagedAndImmune) return;

        currentEnergy -= damageAmount;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateEnergyUI(currentEnergy, maxEnergy);
        }

        TriggerDamageEffect();

        if (currentEnergy <= 0) Die();
    }

    private void TriggerDamageEffect()
    {
        if (damageFlashRoutine != null) StopCoroutine(damageFlashRoutine);
        damageFlashRoutine = StartCoroutine(PerformDamageFlashRoutine());
    }

    private void Die()
    {
        currentEnergy = 0;
        spriteRenderer.color = Color.white;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateEnergyUI(currentEnergy, maxEnergy);
            GameManager.Instance.TriggerGameOver();
        }

        Debug.Log("¡Juego Terminado!");
        gameObject.SetActive(false);
    }

    #endregion

    #region Power-Ups

    public void RestoreEnergy(float amount)
    {
        currentEnergy += amount;
        if (currentEnergy > maxEnergy) currentEnergy = maxEnergy;

        if (GameManager.Instance != null) GameManager.Instance.UpdateEnergyUI(currentEnergy, maxEnergy);
    }

    public void EquipShotgun()
    {
        hasShotgun = true;
        hasAuto = false;
        fireRate = baseFireRate;
        ResetWeaponTimer();
    }

    public void EquipAuto()
    {
        hasAuto = true;
        hasShotgun = false;
        fireRate = baseFireRate / 3f;
        ResetWeaponTimer();
    }

    private void ResetWeaponTimer()
    {
        if (weaponTimerRoutine != null) StopCoroutine(weaponTimerRoutine);
        weaponTimerRoutine = StartCoroutine(WeaponTimerRoutine());
    }

    private IEnumerator WeaponTimerRoutine()
    {
        yield return new WaitForSeconds(powerUpDuration);

        hasShotgun = false;
        hasAuto = false;
        fireRate = baseFireRate;
        Debug.Log("Arma agotada. Volviendo al cañón estándar.");
    }

    #endregion

    #region Utilidades

    private void CalculateScreenBounds()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            minBounds = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 0));
            maxBounds = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, 0));
        }
    }

    #endregion
}