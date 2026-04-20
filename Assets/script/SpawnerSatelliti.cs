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
    public float attesaInizialePrimaDelPrimoSpawn = 3f; // 🔥 NUOVO: attesa prima del primo spawn
    public float attesaDopoDistruzione = 5f;

    [Header("Area Spawn")]
    public float raggioSpawn = 10f;

    [Header("Avviso")]
    public string messaggioAvviso = "CHECKPOINT IN ARRIVO!";
    public float durataAvviso = 3f;

    [Header("Sconfitta")]
    public string nomeScenaPerdita = "LoseScreen";

    [Header("Vittoria")]
    public string nomeScenaWin = "WINscreen";

    private int satelliteSpawnati = 0;
    private int satellitiPresi = 0;
    private bool partitaFinita = false;
    private bool avvisoAttivo = false;

    void Start()
    {
        if (cameraPrincipale == null)
            cameraPrincipale = Camera.main;

        if (testoAvviso != null)
            testoAvviso.gameObject.SetActive(false);

        // 🔥 NON SPAWNARE SUBITO, aspetta l'attesa iniziale
        StartCoroutine(AttesaPrimaSpawn());
    }

    // 🔥 NUOVA COROUTINE: attesa prima del primo spawn
    System.Collections.IEnumerator AttesaPrimaSpawn()
    {
        Debug.Log($"⏳ Attesa di {attesaInizialePrimaDelPrimoSpawn} secondi prima del primo spawn...");
        yield return new WaitForSeconds(attesaInizialePrimaDelPrimoSpawn);

        if (!partitaFinita)
        {
            Debug.Log("🎯 Primo spawn iniziato!");
            SpawnSatellite();
        }
    }

    // Chiamato quando un satellite viene preso
    public void OnSatellitePreso(Satellite satellite)
    {
        if (partitaFinita) return;

        satellitiPresi++;
        Debug.Log($"📊 Satellite preso! ({satellitiPresi}/{maxSatelliti})");

        // 🔥 VITTORIA DOPO IL PRIMO SATELLITE
        if (satellitiPresi >= 1)
        {
            Vittoria();
        }
    }

    // Gestisce la vittoria
    void Vittoria()
    {
        if (partitaFinita) return;

        partitaFinita = true;
        StopAllCoroutines();
        NascondiAvviso();

        Debug.Log("🏆 SATELLITE PRESO! VITTORIA!");

        // Salva il livello attuale
        string currentLevel = SceneManager.GetActiveScene().name;
        PlayerPrefs.SetString("CurrentLevel", currentLevel);
        PlayerPrefs.Save();

        Debug.Log("Salvato livello: " + currentLevel);

        // Vai alla scena di vittoria
        SceneManager.LoadScene(nomeScenaWin);
    }

    void SpawnSatellite()
    {
        if (partitaFinita) return;
        if (satelliteSpawnati >= maxSatelliti) return;
        if (prefabSatellite == null) return;

        Vector2 cerchio = Random.insideUnitCircle * raggioSpawn;
        Vector3 posCasuale = transform.position + new Vector3(cerchio.x, 0f, cerchio.y);

        GameObject obj = Instantiate(prefabSatellite, posCasuale, Quaternion.identity);

        Satellite sat = obj.GetComponent<Satellite>();
        if (sat != null)
            sat.spawner = this;

        satelliteSpawnati++;

        Debug.Log($"🛸 Satellite {satelliteSpawnati}/{maxSatelliti} spawnato!");

        // Mostra l'avviso allo spawn
        MostraAvviso();

        // Avvia il controllo per la visibilità
        StartCoroutine(ControllaVisibilitaPrimaVolta(obj));
        StartCoroutine(AttesaProssimoSpawn(obj));
    }

    System.Collections.IEnumerator ControllaVisibilitaPrimaVolta(GameObject satellite)
    {
        bool avvisoMostratoDaVisibilita = false;

        while (satellite != null && !avvisoMostratoDaVisibilita && !partitaFinita)
        {
            if (IsVisibile(satellite))
            {
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
        while (satellite != null && !partitaFinita)
            yield return null;

        // Se la partita è finita, fermati
        if (partitaFinita) yield break;

        // Se il satellite è stato distrutto (non preso) e abbiamo raggiunto il limite
        if (satelliteSpawnati >= maxSatelliti && satellitiPresi < 1)
        {
            Debug.Log($"❌ Nessun satellite preso su {maxSatelliti} spawn. Game Over!");
            SceneManager.LoadScene(nomeScenaPerdita);
            yield break;
        }

        yield return new WaitForSeconds(attesaDopoDistruzione);

        if (!partitaFinita && satelliteSpawnati < maxSatelliti)
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

        Debug.Log($"📢 Avviso mostrato: {messaggioAvviso}");

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