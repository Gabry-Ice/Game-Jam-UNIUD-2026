using UnityEngine;
using TMPro;

public class SatelliteSpawner : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject prefabSatellite;

    [Header("Riferimenti")]
    public TMP_Text testoAvviso;
    public Camera cameraPrincipale;

    [Header("Spawn")]
    public int maxSatelliti = 3;
    public float attesaDopoDistruzione = 5f;  // secondi dopo la distruzione del satellite

    [Header("Area Spawn")]
    public float raggioX = 10f;
    public float raggioZ = 10f;
    public float altezzaSpawn = 20f;

    [Header("Avviso")]
    public string messaggioAvviso = "CHECKPOINT IN ARRIVO!";
    public float durataAvviso = 3f;

    int satelliteSpawnati = 0;

    void Start()
    {
        if (cameraPrincipale == null)
            cameraPrincipale = Camera.main;

        if (testoAvviso != null)
            testoAvviso.gameObject.SetActive(false);

        // spawna il primo subito
        SpawnSatellite();
    }

    void SpawnSatellite()
    {
        if (satelliteSpawnati >= maxSatelliti) return;
        if (prefabSatellite == null) return;

        Vector3 posCasuale = new Vector3(
            transform.position.x + Random.Range(-raggioX, raggioX),
            altezzaSpawn,
            transform.position.z + Random.Range(-raggioZ, raggioZ)
        );

        GameObject obj = Instantiate(prefabSatellite, posCasuale, Quaternion.identity);
        satelliteSpawnati++;

        StartCoroutine(ControllVisibilita(obj));
        StartCoroutine(AttesaProssimoSpawn(obj));
    }

    System.Collections.IEnumerator AttesaProssimoSpawn(GameObject satellite)
    {
        // aspetta che il satellite venga distrutto
        while (satellite != null)
            yield return null;

        // aspetta 5 secondi dopo la distruzione
        yield return new WaitForSeconds(attesaDopoDistruzione);

        SpawnSatellite();
    }

    System.Collections.IEnumerator ControllVisibilita(GameObject satellite)
    {
        bool avvisoMostrato = false;

        while (satellite != null && !avvisoMostrato)
        {
            if (IsVisibile(satellite))
            {
                avvisoMostrato = true;
                MostraAvviso();
            }
            yield return null;
        }
    }

    bool IsVisibile(GameObject obj)
    {
        if (cameraPrincipale == null || obj == null) return false;

        Plane[] piani = GeometryUtility.CalculateFrustumPlanes(cameraPrincipale);
        Renderer r = obj.GetComponent<Renderer>();

        if (r != null)
            return GeometryUtility.TestPlanesAABB(piani, r.bounds);

        Vector3 viewPos = cameraPrincipale.WorldToViewportPoint(obj.transform.position);
        return viewPos.x >= 0 && viewPos.x <= 1 &&
               viewPos.y >= 0 && viewPos.y <= 1 &&
               viewPos.z > 0;
    }

    void MostraAvviso()
    {
        if (testoAvviso == null) return;

        testoAvviso.text = messaggioAvviso;
        testoAvviso.gameObject.SetActive(true);

        CancelInvoke(nameof(NascondiAvviso));
        Invoke(nameof(NascondiAvviso), durataAvviso);
    }

    void NascondiAvviso()
    {
        if (testoAvviso != null)
            testoAvviso.gameObject.SetActive(false);
    }
}