using UnityEngine;

public class RedSquadronManager : MonoBehaviour
{
    [Header("Configuración del Premio")]
    public GameObject powPrefab;
    public int totalPlanes = 5;

    private int planesKilled = 0;
    private bool squadronEscaped = false;
    private bool premioEntregado = false;

    // El spawner llama esto antes de cada oleada para resetear el contador
    public void ResetSquadron(int cantidadPlanes)
    {
        planesKilled = 0;
        squadronEscaped = false;
        premioEntregado = false;
        totalPlanes = cantidadPlanes;
        Debug.Log("[Squadron] Reset - esperando " + cantidadPlanes + " aviones");
    }

    public void ReportPlaneKilled(Vector3 deathPosition)
    {
        if (squadronEscaped || premioEntregado) return;

        planesKilled++;
        Debug.Log("[Squadron] Avión destruido: " + planesKilled + "/" + totalPlanes);

        if (planesKilled >= totalPlanes)
        {
            premioEntregado = true;
            Debug.Log("[Squadron] ¡Escuadrón aniquilado! Soltando PowerUp");
            if (powPrefab != null)
                Instantiate(powPrefab, deathPosition, Quaternion.identity);
        }
    }

    public void ReportPlaneEscaped()
    {
        squadronEscaped = true;
        Debug.Log("[Squadron] Un avión escapó - premio cancelado");
    }
}