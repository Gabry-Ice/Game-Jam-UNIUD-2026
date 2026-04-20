using UnityEngine;
using System.Collections.Generic;

public class SfondoListaLoop : MonoBehaviour
{
    [Header("Configurazione Oggetti")]
    public List<GameObject> oggettiDaCiclo;
    public float velocita = 5f;

    [Header("Limiti del Loop")]
    public float limiteFineX = 127f; // Punto di sparizione

    [Header("Rientro Remoto")]
    // Per farli partire "ancora più indietro", spingiamo questi valori
    // verso numeri negativi molto alti (es. -200, -500)
    public float rientroMinX = -150f;
    public float rientroMaxX = -400f;

    void Update()
    {
        for (int i = 0; i < oggettiDaCiclo.Count; i++)
        {
            GameObject obj = oggettiDaCiclo[i];

            if (obj != null)
            {
                // Muove l'oggetto verso destra
                obj.transform.position += Vector3.right * velocita * Time.deltaTime;

                // Se supera il limite a destra...
                if (obj.transform.position.x >= limiteFineX)
                {
                    ResetOggetto(obj);
                }
            }
        }
    }

    void ResetOggetto(GameObject obj)
    {
        // Sceglie un punto molto indietro nel tempo/spazio
        float partenzaCasualeX = Random.Range(rientroMinX, rientroMaxX);

        // Mantiene le coordinate Y e Z originali dell'oggetto
        obj.transform.position = new Vector3(
            partenzaCasualeX,
            obj.transform.position.y,
            obj.transform.position.z
        );
    }
}