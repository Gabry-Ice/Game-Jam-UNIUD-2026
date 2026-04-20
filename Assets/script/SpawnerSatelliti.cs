using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // <--- Necessario per caricare le scene

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
    public float raggioSpawn = 10f;

    [Header("Avviso")]
    public string messaggioAvviso = "CHECKPOINT IN ARRIVO!";
    public float durataAvviso = 3f;

    [Header("Sconfitta")]
    public string nomeScenaPerdita = "LoseScreen"; // Il nome della tua scena di Game Over

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
    }

    void SpawnSatellite()
    {
        if (checkpointPreso) return;

        // Se abbiamo già spawnato il numero massimo, non ne creiamo altri
        if (satelliteSpawnati >= maxSatelliti) return;

        if (prefabSatellite == null) return;

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
        // Aspetta che il satellite appena spawnato venga distrutto
        while (satellite != null)
            yield return null;

        // Se il giocatore ha preso il checkpoint, fermati
        if (checkpointPreso) yield break;

        // Se il satellite è stato distrutto (ma non preso) e abbiamo raggiunto il limite
        if (satelliteSpawnati >= maxSatelliti)
        {
            Debug.Log("Terzo satellite perso. Caricamento scena sconfitta...");
            SceneManager.LoadScene(nomeScenaPerdita);
            yield break;
        }

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