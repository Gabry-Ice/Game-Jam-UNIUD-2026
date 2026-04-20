using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

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
    public string nomeScenaPerdita = "LoseScreen";

    int satelliteSpawnati = 0;
    bool checkpointPreso = false;
    bool avvisoAttivo = false;

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

        string currentLevel = SceneManager.GetActiveScene().name;
        PlayerPrefs.SetString("CurrentLevel", currentLevel);

        Debug.Log("Salvato livello: " + currentLevel);

        SceneManager.LoadScene("WINscreen");
    }

    void SpawnSatellite()
    {
        if (checkpointPreso) return;
        if (satelliteSpawnati >= maxSatelliti) return;
        if (prefabSatellite == null) return;

        Vector2 cerchio = Random.insideUnitCircle * raggioSpawn;
        Vector3 posCasuale = transform.position + new Vector3(cerchio.x, 0f, cerchio.y);

        GameObject obj = Instantiate(prefabSatellite, posCasuale, Quaternion.identity);

        Satellite sat = obj.GetComponent<Satellite>();
        if (sat != null)
            sat.spawner = this;

        satelliteSpawnati++;

        // 🔥 MOSTRA L'AVVISO ALLO SPAWN
        MostraAvviso();

        // 🔥 AVVIA IL CONTROLLO PER LA VISIBILITÀ (per mostrarlo di nuovo se necessario)
        StartCoroutine(ControllaVisibilitaPrimaVolta(obj));
        StartCoroutine(AttesaProssimoSpawn(obj));
    }

    // 🔥 NUOVA COROUTINE: controlla la visibilità solo una volta
    System.Collections.IEnumerator ControllaVisibilitaPrimaVolta(GameObject satellite)
    {
        bool avvisoMostratoDaVisibilita = false;

        while (satellite != null && !avvisoMostratoDaVisibilita)
        {
            if (IsVisibile(satellite))
            {
                // Mostra l'avviso se non è già attivo
                if (!avvisoAttivo)
                {
                    MostraAvviso();
                }
                avvisoMostratoDaVisibilita = true;
            }
            yield return null;
        }
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
        if (avvisoAttivo) return;

        avvisoAttivo = true;
        testoAvviso.text = messaggioAvviso;
        testoAvviso.gameObject.SetActive(true);

        Debug.Log($"📢 Avviso mostrato (spawn o visibilità): {messaggioAvviso}");

        // Annulla eventuali invoke precedenti
        CancelInvoke(nameof(NascondiAvviso));
        Invoke(nameof(NascondiAvviso), durataAvviso);
    }

    void NascondiAvviso()
    {
        if (testoAvviso != null)
            testoAvviso.gameObject.SetActive(false);

        avvisoAttivo = false;
        Debug.Log("Avviso nascosto");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, raggioSpawn);
    }
}