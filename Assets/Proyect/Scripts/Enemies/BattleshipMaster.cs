using UnityEngine;

public class BattleshipMaster : MonoBehaviour
{
    public int torretasVivas;
    private bool hundido = false;

    void OnEnable()
    {
        hundido = false;
        // Contamos cuántas torretas hay activas al aparecer
        torretasVivas = GetComponentsInChildren<TurretEnemy>().Length;
        Debug.Log("Acorazado activo. Torretas detectadas: " + torretasVivas);
    }

    // Esta función es llamada por el script Enemy de las torretas
    public void TorretaDestruida()
    {
        if (hundido) return;

        torretasVivas--;
        Debug.Log("Torreta eliminada. Quedan: " + torretasVivas);

        if (torretasVivas <= 0)
        {
            HundirBarco();
        }
    }

    void HundirBarco()
    {
        hundido = true;
        Debug.Log("Hundiendo Acorazado...");

        // Aquí podrías disparar una explosión grande antes de desactivar
        // ObjectPooler.Instance.SpawnFromPool("ExplosionGrande", transform.position, Quaternion.identity);

        gameObject.SetActive(false);
    }
}