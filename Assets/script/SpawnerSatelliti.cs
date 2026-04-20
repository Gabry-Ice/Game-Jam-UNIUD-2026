using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class SatelliteSpawner : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject prefabSatellite;

    [Header("Riferimenti")]
    public TMP_Text testoAvviso;
    public Camera cameraPrincipale;

    [Header("Spawn")]
    public int maxSatelliti = 3;
    private float attesaIniziale = 30f;
    public float attesaDopoDistruzione = 5f;

    [Header("Area Spawn")]
    public float raggioSpawn = 10f;

    [Header("Avviso")]
    public string messaggioAvviso = "CHECKPOINT IN ARRIVO!";
    public float durataAvviso = 3f;

    [Header("Scene")]
    public string nomeScenaPerdita = "LoseScreen";
    public string nomeScenaWin;

    private int satelliteSpawnati = 0;
    private bool partitaFinita = false;
    private GameObject satelliteCorrente = null;

    void Start()
    {
        if (cameraPrincipale == null)
            cameraPrincipale = Camera.main;

        if (testoAvviso != null)
            testoAvviso.gameObject.SetActive(false);

        StartCoroutine(LoopSpawn());
    }

    IEnumerator LoopSpawn()
    {
        yield return new WaitForSeconds(attesaIniziale);

        while (!partitaFinita && satelliteSpawnati < maxSatelliti)
        {
            SpawnSatellite();

            // Aspetta che il satellite corrente venga distrutto
            yield return new WaitUntil(() => satelliteCorrente == null || partitaFinita);

            if (partitaFinita) yield break;

            // Satellite distrutto senza essere preso
            Debug.Log($"💥 Satellite distrutto ({satelliteSpawnati}/{maxSatelliti})");

            if (satelliteSpawnati >= maxSatelliti)
            {
                // Esauriti tutti i satelliti senza vittoria
                StartCoroutine(CaricaScena(nomeScenaPerdita));
                yield break;
            }

            yield return new WaitForSeconds(attesaDopoDistruzione);
        }
    }

    void SpawnSatellite()
    {
        if (prefabSatellite == null) return;

        Vector2 cerchio = Random.insideUnitCircle.normalized * raggioSpawn;
        Vector3 posizione = transform.position + new Vector3(cerchio.x, 0f, cerchio.y);

        satelliteCorrente = Instantiate(prefabSatellite, posizione, Quaternion.identity);
        satelliteSpawnati++;

        Satellite sat = satelliteCorrente.GetComponent<Satellite>();
        if (sat != null)
            sat.spawner = this;

        Debug.Log($"🛸 Satellite {satelliteSpawnati}/{maxSatelliti} spawnato");

        MostraAvviso();
        StartCoroutine(AvvisoSeVisibile());
    }

    // Mostra l'avviso anche quando il satellite entra nel campo visivo
    IEnumerator AvvisoSeVisibile()
    {
        while (satelliteCorrente != null && !partitaFinita)
        {
            if (IsVisibile(satelliteCorrente))
            {
                MostraAvviso();
                yield break;
            }
            yield return null;
        }
    }

    // Chiamato da Satellite.cs quando viene preso
    public void OnSatellitePreso()
    {
        if (partitaFinita) return;

        partitaFinita = true;
        satelliteCorrente = null;

        StopAllCoroutines();
        NascondiAvviso();

        Debug.Log("🏆 VITTORIA!");

        string currentLevel = SceneManager.GetActiveScene().name;
        PlayerPrefs.SetString("CurrentLevel", currentLevel);
        PlayerPrefs.Save();

        StartCoroutine(CaricaScena(nomeScenaWin));
    }

    IEnumerator CaricaScena(string nomeScena)
    {
        yield return new WaitForSeconds(0.1f);
        SceneManager.LoadScene(nomeScena);
    }

    void MostraAvviso()
    {
        if (testoAvviso == null || testoAvviso.gameObject.activeSelf) return;

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

    bool IsVisibile(GameObject obj)
    {
        if (cameraPrincipale == null || obj == null) return false;

        Renderer r = obj.GetComponent<Renderer>();
        if (r != null)
        {
            Plane[] piani = GeometryUtility.CalculateFrustumPlanes(cameraPrincipale);
            return GeometryUtility.TestPlanesAABB(piani, r.bounds);
        }

        Vector3 viewPos = cameraPrincipale.WorldToViewportPoint(obj.transform.position);
        return viewPos.x >= 0 && viewPos.x <= 1 &&
               viewPos.y >= 0 && viewPos.y <= 1 &&
               viewPos.z > 0;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, raggioSpawn);
    }
}