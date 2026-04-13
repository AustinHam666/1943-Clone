using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement; // Necesario para reiniciar o cambiar de escenas

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Referencias de UI (HUD)")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Slider energySlider;

    [Header("Pantallas de Menú")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject gameOverPanel;

    private int currentScore = 0;
    private bool isPaused = false;
    private bool isGameOver = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        UpdateScoreUI();

        // Aseguramos que el tiempo corra normal y los paneles estén ocultos al iniciar
        Time.timeScale = 1f;
        if (pausePanel != null) pausePanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    #region Manejo de Puntos y Energía

    public void AddScore(int points)
    {
        if (isGameOver) return;
        currentScore += points;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null) scoreText.text = "SCORE: " + currentScore.ToString("D6");
    }

    public void UpdateEnergyUI(float currentEnergy, float maxEnergy)
    {
        if (energySlider != null) energySlider.value = currentEnergy / maxEnergy;
    }

    #endregion

    #region Estados del Juego (Pausa y Game Over)

    public void TogglePause()
    {
        // Si ya perdimos, no podemos pausar/despausar
        if (isGameOver) return;

        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f; // Congela el motor de físicas y el tiempo
            if (pausePanel != null) pausePanel.SetActive(true);
        }
        else
        {
            ResumeGame();
        }
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; // Descongela el juego
        if (pausePanel != null) pausePanel.SetActive(false);
    }

    public void TriggerGameOver()
    {
        isGameOver = true;
        Time.timeScale = 0f; // Detiene el juego
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
    }

    #endregion

    #region Funciones para los Botones (UI)

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        // Recarga la escena actual
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        // Asumimos que la Escena 0 en el Build Settings será el Menú Principal
        SceneManager.LoadScene(0);
    }

    #endregion
}