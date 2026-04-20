using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // 🔥 AGGIUNTO

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
    public float raggioSpawn = 10f; // distanza massima dal GameObject vuoto

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
        StopAllCoroutines();
        NascondiAvviso();
        Debug.Log("Checkpoint preso – nessun altro spawn.");

        // 🔥 SALVA IL LIVELLO ATTUALE
        string currentLevel = SceneManager.GetActiveScene().name;
        PlayerPrefs.SetString("CurrentLevel", currentLevel);

        Debug.Log("Salvato livello: " + currentLevel);

        // 🔥 VAI ALLA WIN SCREEN
        SceneManager.LoadScene("WINscreen");
    }

    void SpawnSatellite()
    {
        if (checkpointPreso) return;
        if (satelliteSpawnati >= maxSatelliti) return;
        if (prefabSatellite == null) return;

        // Punto casuale nel raggio attorno al GameObject vuoto
        Vector2 cerchio = Random.insideUnitCircle * raggioSpawn;
        Vector3 posCasuale = transform.position + new Vector3(cerchio.x, 0f, cerchio.y);

        GameObject obj = Instantiate(prefabSatellite, posCasuale, Quaternion.identity);

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

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, raggioSpawn);
    }
}
