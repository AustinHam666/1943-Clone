using UnityEngine;

public class RedSquadronManager : MonoBehaviour
{
    [Header("Configuración del Premio")]
    [Tooltip("Arrastrá acá tu prefab ItemPow")]
    public GameObject powPrefab;
    public int totalPlanes = 5;

    private int planesKilled = 0;
    private bool squadronEscaped = false;

    // Los aviones rojos llaman a esta función cuando tu bala los destruye
    public void ReportPlaneKilled(Vector3 deathPosition)
    {
        if (squadronEscaped)
        {
            Debug.Log("Premio cancelado: ¡Un avión se escapó!");
            return;
        }

        planesKilled++;
        Debug.Log("Avión rojo destruido. Llevamos: " + planesKilled + " de " + totalPlanes);

        if (planesKilled >= totalPlanes)
        {
            Debug.Log("¡Escuadrón aniquilado! Soltando Power-Up en: " + deathPosition);
            if (powPrefab != null) Instantiate(powPrefab, deathPosition, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    // Los aviones rojos llaman a esta función si tocan el límite de la pantalla
    public void ReportPlaneEscaped()
    {
        squadronEscaped = true;
    }
}