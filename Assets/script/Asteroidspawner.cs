using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    [Header("Prefab Asteroidi")]
    public GameObject[] prefabAsteroidi = new GameObject[3];

    [Header("Prefab Astronavi")]
    public GameObject[] prefabAstronavi = new GameObject[3];

    [Header("Spawn")]
    public float intervalloSpawn = 2f;
    public float raggioSpawn = 10f;      // quanto fuori dalla telecamera spawnare

    [Header("Sicurezza")]
    public Transform nave;
    public float raggioDiSicurezza = 5f;
    public int maxTentativi = 10;

    [Header("Difficolta")]
    public float difficolta = 1f;
    public float intervalloMinimo = 0.3f;
    public float moltiplicatoreDifficolta = 0.1f;

    [Header("Riferimenti")]
    public Camera cameraPrincipale;

    float timer;
    float intervalloCorrente;

    void Start()
    {
        if (cameraPrincipale == null)
            cameraPrincipale = Camera.main;

        AggiornaDifficolta();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= intervalloCorrente)
        {
            timer = 0f;
            SpawnAsteroide();
        }
    }

    public void ImpostaDifficolta(float nuovaDifficolta)
    {
        difficolta = nuovaDifficolta;
        AggiornaDifficolta();
    }

    void AggiornaDifficolta()
    {
        intervalloCorrente = Mathf.Max(
            intervalloMinimo,
            intervalloSpawn - (difficolta * moltiplicatoreDifficolta)
        );
    }

    Vector3 GetPosizioneeFuoriCamera()
    {
        float distanza = Mathf.Abs(cameraPrincipale.transform.position.y - nave.position.y);

        // calcola i bordi della camera in world space
        Vector3 bottomLeft = cameraPrincipale.ViewportToWorldPoint(new Vector3(0, 0, distanza));
        Vector3 topRight = cameraPrincipale.ViewportToWorldPoint(new Vector3(1, 1, distanza));

        float minX = bottomLeft.x;
        float maxX = topRight.x;
        float minZ = bottomLeft.z;
        float maxZ = topRight.z;

        // scegli uno dei 4 lati casualmente
        int lato = Random.Range(0, 4);
        float x, z;

        switch (lato)
        {
            case 0: // sinistra
                x = minX - Random.Range(1f, raggioSpawn);
                z = Random.Range(minZ, maxZ);
                break;
            case 1: // destra
                x = maxX + Random.Range(1f, raggioSpawn);
                z = Random.Range(minZ, maxZ);
                break;
            case 2: // basso
                x = Random.Range(minX, maxX);
                z = minZ - Random.Range(1f, raggioSpawn);
                break;
            default: // alto
                x = Random.Range(minX, maxX);
                z = maxZ + Random.Range(1f, raggioSpawn);
                break;
        }

        return new Vector3(x, nave.position.y, z);
    }

    void SpawnAsteroide()
    {
        Debug.Log("Spawnato asteroide");

        GameObject[] pool = Random.value > 0.5f ? prefabAsteroidi : prefabAstronavi;

        if (pool.Length == 0)
        {
            Debug.Log("Pool vuoto");
            return;
        }

        int indice = Random.Range(0, pool.Length);
        GameObject prefab = pool[indice];

        if (prefab == null) return;

        for (int i = 0; i < maxTentativi; i++)
        {
            Vector3 spawnPos = GetPosizioneeFuoriCamera();
            float distanza = nave == null ? -1f : Vector3.Distance(spawnPos, nave.position);
            Debug.Log($"Tentativo {i}: posizione {spawnPos}, distanza nave {distanza}");

            if (nave == null || Vector3.Distance(spawnPos, nave.position) >= raggioDiSicurezza)
            {
                Debug.Log($"Spawn riuscito in {spawnPos}");
                Instantiate(prefab, spawnPos, Quaternion.identity);
                return;
            }
        }

        Debug.Log("Spawn saltato: nessuna posizione libera trovata.");
    }
}