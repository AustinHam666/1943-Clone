using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Necesario para el Slider

public class MenuManager : MonoBehaviour
{
    [Header("Configuración del Cursor Retro")]
    [SerializeField] private RectTransform indicator; // Tu triangulito
    [SerializeField] private float xOffset = -120f;    // Distancia a la izquierda

    [Header("Paneles del Menú")]
    public GameObject panelPrincipal;
    public GameObject panelOpciones;
    public GameObject panelNiveles;

    [Header("Ajustes de Sonido")]
    [SerializeField] private Slider volumenSlider;

    private void Start()
    {
        // 1. Configuración del Cursor
        if (indicator != null) indicator.gameObject.SetActive(false);

        // 2. Configuración del Volumen
        // Cargamos el volumen guardado (si no existe, por defecto es 0.5f)
        float volumenGuardado = PlayerPrefs.GetFloat("VolumenGeneral", 0.5f);

        // Aplicamos el volumen al sistema de Unity
        AudioListener.volume = volumenGuardado;

        // Configuramos el Slider si está asignado
        if (volumenSlider != null)
        {
            volumenSlider.value = volumenGuardado;
            // Vinculamos el cambio del slider con la función ActualizarVolumen
            volumenSlider.onValueChanged.AddListener(ActualizarVolumen);
        }
    }

    // --- LÓGICA DEL VOLUMEN ---

    public void ActualizarVolumen(float valor)
    {
        // Cambia el volumen maestro de Unity (afecta a todos los AudioSources)
        AudioListener.volume = valor;

        // Guarda el valor en el disco para la próxima vez
        PlayerPrefs.SetFloat("VolumenGeneral", valor);
    }

    // --- LÓGICA DEL INDICADOR (LLAMADA POR LOS BOTONES Y SLIDER) ---

    public void SetCursorAt(RectTransform uiElementRect)
    {
        if (indicator == null) return;

        // Activamos el cursor y lo movemos a la posición del elemento con el offset
        indicator.gameObject.SetActive(true);
        Vector3 newPos = uiElementRect.position;
        newPos.x += xOffset;
        indicator.position = newPos;
    }

    // --- FUNCIONES DEL MENÚ PRINCIPAL ---

    public void AbrirSeleccionNiveles()
    {
        panelPrincipal.SetActive(false);
        panelNiveles.SetActive(true);
        OcultarCursor();
    }

    public void AbrirOpciones()
    {
        panelPrincipal.SetActive(false);
        panelOpciones.SetActive(true);
        OcultarCursor();
    }

    public void SalirDelJuego()
    {
        Application.Quit();
    }

    // --- FUNCIONES DE SELECCIÓN DE NIVEL ---

    public void CargarNivel1()
    {
        SceneManager.LoadScene("Nivel1");
    }

    public void CargarNivel2()
    {
        SceneManager.LoadScene("Nivel2");
    }

    public void VolverAlMenuPrincipal()
    {
        panelNiveles.SetActive(false);
        panelOpciones.SetActive(false);
        panelPrincipal.SetActive(true);
        OcultarCursor();
    }

    private void OcultarCursor()
    {
        if (indicator != null) indicator.gameObject.SetActive(false);
    }
}