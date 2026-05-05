using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuVictoria : MonoBehaviour
{
    [Header("Textos")]
    public TextMeshProUGUI textoPuntuacionFinal;

    private void OnEnable()
    {
        // 1. Pausamos el juego
        Time.timeScale = 0f;

        // 2. Buscamos el puntaje en tu GameManager
        if (GameManager.Instance != null)
        {
            // Usamos "currentScore" que es como se llama en tu script
            textoPuntuacionFinal.text = "Puntuacion Final: " + GameManager.Instance.currentScore.ToString("D5");
        }
    }

    public void IrSiguienteNivel()
    {
        Time.timeScale = 1f;
        // Asegurate de que la escena se llame exactamente "Nivel2"
        SceneManager.LoadScene("Nivel2");
    }

    public void IrMenuPrincipal()
    {
        Time.timeScale = 1f;
        // Tu GameManager asume que el menú es el index 0. Usamos LoadScene(0) por seguridad.
        SceneManager.LoadScene(0);
    }
}