using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Paneles del Menú")]
    public GameObject panelPrincipal;
    public GameObject panelOpciones;
    public GameObject panelNiveles; // Nuevo panel

    // --- FUNCIONES DEL MENÚ PRINCIPAL ---

    public void AbrirSeleccionNiveles()
    {
        // En lugar de cargar el nivel, abrimos el sub-menú
        panelPrincipal.SetActive(false);
        panelNiveles.SetActive(true);
    }

    public void AbrirOpciones()
    {
        panelPrincipal.SetActive(false);
        panelOpciones.SetActive(true);
    }

    public void SalirDelJuego()
    {
        Application.Quit();
    }

    // --- FUNCIONES DE SELECCIÓN DE NIVEL ---

    public void CargarNivel1()
    {
        SceneManager.LoadScene("Nivel1"); // Asegurate que se llame así en el Build Settings
    }

    public void CargarNivel2()
    {
        SceneManager.LoadScene("Nivel2"); // Asegurate que se llame así en el Build Settings
    }

    public void VolverAlMenuPrincipal()
    {
        // Esta función sirve para volver desde Opciones O desde Selección de Niveles
        panelNiveles.SetActive(false);
        panelOpciones.SetActive(false);
        panelPrincipal.SetActive(true);
    }
}