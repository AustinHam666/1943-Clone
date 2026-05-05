using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Esto crea el "Molde" para nuestra tabla en el Inspector
[System.Serializable]
public class EventoDeNivel
{
    public string descripcion = "Ej: Salen los Verdes"; // Para que vos te organices
    public float segundoExacto; // En qué segundo del nivel querés que pase
    public GameObject objetoAActivar; // El Spawner, la Nodriza o el Boss
}

public class LevelManager : MonoBehaviour
{
    [Header("Línea de Tiempo del Nivel")]
    [Tooltip("Añadí elementos con el botón + para armar tu nivel")]
    public List<EventoDeNivel> lineaDeTiempo;

    void Start()
    {
        // 1. Apagamos TODO lo que esté en la lista por seguridad
        foreach (var evento in lineaDeTiempo)
        {
            if (evento.objetoAActivar != null)
                evento.objetoAActivar.SetActive(false);
        }

        // 2. Arrancamos el reloj
        StartCoroutine(EjecutarNivel());
    }

    IEnumerator EjecutarNivel()
    {
        float cronometro = 0f;

        // Repasamos la lista evento por evento
        foreach (var evento in lineaDeTiempo)
        {
            // Calculamos cuánto falta esperar para este evento
            float tiempoEspera = evento.segundoExacto - cronometro;

            if (tiempoEspera > 0)
            {
                yield return new WaitForSeconds(tiempoEspera);
                cronometro += tiempoEspera; // Actualizamos nuestro reloj interno
            }

            // ¡Llegó la hora! Activamos el spawner o enemigo
            if (evento.objetoAActivar != null)
            {
                evento.objetoAActivar.SetActive(true);
                Debug.Log("Activando: " + evento.descripcion + " en el segundo " + cronometro);
            }
        }
    }
}