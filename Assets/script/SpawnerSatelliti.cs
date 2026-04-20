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
    public float attesaDopoDistruzione = 5f;

    [Header("Area Spawn")]
    public float raggioX = 10f;
    public float raggioZ = 10f;
    public float altezzaSpawn = 20f;

    [Header("Avviso")]
    public string messaggioAvviso = "CHECKPOINT IN ARRIVO!";
    public float durataAvviso = 3f;

    int satelliteSpawnati = 0;
    bool checkpointPreso = false;

    void Start()
    {
        if (cameraPrincipale == null)
            cameraPrincipale = Camera.main;

        if (testoAvviso != null)
            testoAvviso.gameObject.SetActive(false);

        SpawnSatellite();
    }

    public void OnCheckpointRaccolto()
    {
        checkpointPreso = true;
        StopAllCoroutines(); // ← ferma TUTTO immediatamente
        NascondiAvviso();
        Debug.Log("Checkpoint preso – nessun altro spawn.");
    }

    void SpawnSatellite()
    {
        if (checkpointPreso) return;
        if (satelliteSpawnati >= maxSatelliti) return;
        if (prefabSatellite == null) return;
        if (cameraPrincipale == null) return;

        float distanza = Mathf.Abs(cameraPrincipale.transform.position.y - altezzaSpawn);
        Vector3 puntoViewport = new Vector3(
            Random.Range(0.1f, 0.9f),
            Random.Range(0.1f, 0.9f),
            distanza
        );

        Vector3 posCasuale = cameraPrincipale.ViewportToWorldPoint(puntoViewport);
        posCasuale.y = altezzaSpawn;

        GameObject obj = Instantiate(prefabSatellite, posCasuale, Quaternion.identity);

        // Passa il riferimento diretto allo spawner al satellite
        Satellite sat = obj.GetComponent<Satellite>();
        if (sat != null)
            sat.spawner = this;

        satelliteSpawnati++;

        StartCoroutine(ControllVisibilita(obj));
        StartCoroutine(AttesaProssimoSpawn(obj));
    }

    System.Collections.IEnumerator AttesaProssimoSpawn(GameObject satellite)
    {
        while (satellite != null)
            yield return null;

        if (checkpointPreso) yield break;

        yield return new WaitForSeconds(attesaDopoDistruzione);

        if (!checkpointPreso)
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