using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [System.Serializable]
    public class Oleada
    {
        public string nombre = "Oleada";
        public float segundoDeAparicion = 5f;
        public GameObject spawnerObject;
    }

    [Header("Oleadas del nivel")]
    public List<Oleada> oleadas;

    [Header("Boss")]
    public GameObject bossGameObject;

    public static int EnemigosVivos = 0;
    private bool bossActivado = false;
    private bool nivelTerminado = false;

    void Start()
    {
        EnemigosVivos = 0;
        bossActivado = false;
        nivelTerminado = false;

        if (bossGameObject != null)
            bossGameObject.SetActive(false);

        foreach (var oleada in oleadas)
            if (oleada.spawnerObject != null)
                oleada.spawnerObject.SetActive(false);

        StartCoroutine(EjecutarNivel());
    }

    void Update()
    {
        if (!bossActivado && nivelTerminado && EnemigosVivos <= 0)
        {
            bossActivado = true;
            ActivarBoss();
        }
    }

    IEnumerator EjecutarNivel()
    {
        float cronometro = 0f;

        foreach (var oleada in oleadas)
        {
            float espera = oleada.segundoDeAparicion - cronometro;
            if (espera > 0f)
            {
                yield return new WaitForSeconds(espera);
                cronometro = oleada.segundoDeAparicion;
            }

            if (oleada.spawnerObject != null)
            {
                // Apagamos y prendemos para forzar el OnEnable aunque sea el mismo objeto
                oleada.spawnerObject.SetActive(false);
                yield return null;
                oleada.spawnerObject.SetActive(true);
                Debug.Log("[LevelManager] Activando: " + oleada.nombre + " en segundo " + cronometro);
            }
        }

        nivelTerminado = true;
        Debug.Log("[LevelManager] Todas las oleadas lanzadas.");
    }

    void ActivarBoss()
    {
        if (bossGameObject != null)
        {
            bossGameObject.SetActive(true);
            Debug.Log("[LevelManager] BOSS ACTIVADO");
        }
    }

    public static void ReportarMuerte()
    {
        EnemigosVivos--;
        if (EnemigosVivos < 0) EnemigosVivos = 0;
    }

    public static void ReportarSpawn()
    {
        EnemigosVivos++;
    }
}