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

    [Header("Sistema de Energía")]
    [SerializeField] private float maxEnergy = 100f;
    [SerializeField] private float passiveDrainRate = 1.5f;

    [Header("Sistema de Disparo")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.15f;
    [SerializeField] private float shotgunSpreadAngle = 15f;
    [SerializeField] private float powerUpDuration = 10f; // Tiempo que dura el arma especial

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
    private float baseFireRate; // Para recordar nuestra velocidad de disparo original
    private Coroutine weaponTimerRoutine; // Para controlar el temporizador

    public float CurrentEnergy => currentEnergy;
    public int CurrentEvades => currentEvades;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    private void Start()
    {
        currentEnergy = maxEnergy;
        currentEvades = maxEvades;
        baseFireRate = fireRate; // Guardamos la cadencia de fuego inicial
        CalculateScreenBounds();

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
            // Bala central (siempre se dispara)
            ObjectPooler.Instance.SpawnFromPool("PlayerBullet", firePoint.position, firePoint.rotation);

            // Si recogimos el ítem Escopeta, disparamos dos balas extra en diagonal
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
        if (isEvading) return;

        currentEnergy -= damageAmount;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateEnergyUI(currentEnergy, maxEnergy);
        }

        if (currentEnergy <= 0) Die();
    }

    private void Die()
    {
        currentEnergy = 0;

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
        fireRate = baseFireRate / 3f; // Dispara mucho más rápido
        ResetWeaponTimer();
    }

    private void ResetWeaponTimer()
    {
        // Si ya había un temporizador corriendo, lo detenemos para reiniciarlo
        if (weaponTimerRoutine != null) StopCoroutine(weaponTimerRoutine);
        weaponTimerRoutine = StartCoroutine(WeaponTimerRoutine());
    }

    private IEnumerator WeaponTimerRoutine()
    {
        yield return new WaitForSeconds(powerUpDuration);

        // Al terminar el tiempo, volvemos a la normalidad
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