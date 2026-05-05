using UnityEngine;

public class RelojBasico : MonoBehaviour
{
    [Header("Arrastrá acá tus cosas y poné a qué segundo salen")]

    public GameObject spawnerVerdes;
    public float saleA_Los_Segundos_Verdes = 3f;

    public GameObject spawnerRojos;
    public float saleA_Los_Segundos_Rojos = 10f;

    public GameObject spawnerShoryu;
    public float saleA_Los_Segundos_Shoryu = 20f;

    public GameObject laNodriza;
    public float saleA_Los_Segundos_Nodriza = 35f;

    void Start()
    {
        // 1. Apagamos todo de un martillazo por si te olvidaste
        if (spawnerVerdes) spawnerVerdes.SetActive(false);
        if (spawnerRojos) spawnerRojos.SetActive(false);
        if (spawnerShoryu) spawnerShoryu.SetActive(false);
        if (laNodriza) laNodriza.SetActive(false);

        // 2. Ponemos las alarmas
        if (spawnerVerdes) Invoke("PrenderVerdes", saleA_Los_Segundos_Verdes);
        if (spawnerRojos) Invoke("PrenderRojos", saleA_Los_Segundos_Rojos);
        if (spawnerShoryu) Invoke("PrenderShoryu", saleA_Los_Segundos_Shoryu);
        if (laNodriza) Invoke("PrenderNodriza", saleA_Los_Segundos_Nodriza);
    }

    // 3. Cuando suena la alarma, se prende
    void PrenderVerdes() { spawnerVerdes.SetActive(true); }
    void PrenderRojos() { spawnerRojos.SetActive(true); }
    void PrenderShoryu() { spawnerShoryu.SetActive(true); }
    void PrenderNodriza() { laNodriza.SetActive(true); }
}